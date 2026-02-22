using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InfosecAcademyBudgetManagement.Data;
using InfosecAcademyBudgetManagement.Models;

namespace InfosecAcademyBudgetManagement.Controllers
{
    [Authorize]
    public class BudgetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BudgetController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var plans = await _context.BudgetPlans
                .Where(b => !b.IsDeleted)
                .OrderByDescending(b => b.Year)
                .ThenByDescending(b => b.VersionNo)
                .ToListAsync();

            return View(plans);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Year")] BudgetPlan plan)
        {
            if (!ModelState.IsValid)
            {
                return View(plan);
            }

            var now = DateTime.UtcNow;
            var maxVersion = await _context.BudgetPlans
                .Where(b => b.Year == plan.Year && !b.IsDeleted)
                .Select(b => (int?)b.VersionNo)
                .MaxAsync() ?? 0;
            plan.VersionNo = maxVersion + 1;
            plan.CreatedAt = now;
            plan.UpdatedAt = now;

            _context.BudgetPlans.Add(plan);
            await _context.SaveChangesAsync();

            var categories = await _context.Categories.Where(c => !c.IsDeleted).ToListAsync();
            var lines = new List<BudgetLine>();
            foreach (var category in categories)
            {
                for (var month = 1; month <= 12; month++)
                {
                    lines.Add(new BudgetLine
                    {
                        BudgetPlanId = plan.Id,
                        CategoryId = category.Id,
                        Month = month,
                        PlannedAmount = 0m,
                        CreatedAt = now,
                        UpdatedAt = now,
                        IsDeleted = false
                    });
                }
            }

            _context.BudgetLines.AddRange(lines);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = plan.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var plan = await _context.BudgetPlans
                .Include(b => b.Lines)
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

            if (plan is null)
            {
                return NotFound();
            }

            var categories = await _context.Categories.Where(c => !c.IsDeleted).OrderBy(c => c.Name).ToListAsync();
            var now = DateTime.UtcNow;

            foreach (var category in categories)
            {
                for (var month = 1; month <= 12; month++)
                {
                    var exists = plan.Lines.Any(l => l.CategoryId == category.Id && l.Month == month && !l.IsDeleted);
                    if (!exists)
                    {
                        plan.Lines.Add(new BudgetLine
                        {
                            BudgetPlanId = plan.Id,
                            CategoryId = category.Id,
                            Month = month,
                            PlannedAmount = 0m,
                            CreatedAt = now,
                            UpdatedAt = now,
                            IsDeleted = false
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();

            var lines = plan.Lines
                .Where(l => !l.IsDeleted)
                .OrderBy(l => l.CategoryId)
                .ThenBy(l => l.Month)
                .Select(l => new BudgetLineInput
                {
                    CategoryId = l.CategoryId,
                    Month = l.Month,
                    PlannedAmount = l.PlannedAmount
                })
                .ToList();

            var model = new BudgetPlanEditViewModel
            {
                PlanId = plan.Id,
                Year = plan.Year,
                Categories = categories,
                Lines = lines
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BudgetPlanEditViewModel model)
        {
            if (id != model.PlanId)
            {
                return NotFound();
            }

            var plan = await _context.BudgetPlans
                .Include(b => b.Lines)
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

            if (plan is null)
            {
                return NotFound();
            }

            var now = DateTime.UtcNow;
            foreach (var input in model.Lines)
            {
                var line = plan.Lines.FirstOrDefault(l => l.CategoryId == input.CategoryId && l.Month == input.Month && !l.IsDeleted);
                if (line is null)
                {
                    line = new BudgetLine
                    {
                        BudgetPlanId = plan.Id,
                        CategoryId = input.CategoryId,
                        Month = input.Month,
                        CreatedAt = now,
                        UpdatedAt = now,
                        IsDeleted = false
                    };
                    plan.Lines.Add(line);
                }

                line.PlannedAmount = input.PlannedAmount;
                line.UpdatedAt = now;
            }

            plan.UpdatedAt = now;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Report), new { year = plan.Year });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloneVersion(int id)
        {
            var sourcePlan = await _context.BudgetPlans
                .Include(b => b.Lines)
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
            if (sourcePlan is null)
            {
                return NotFound();
            }

            var now = DateTime.UtcNow;
            var maxVersion = await _context.BudgetPlans
                .Where(b => b.Year == sourcePlan.Year && !b.IsDeleted)
                .Select(b => (int?)b.VersionNo)
                .MaxAsync() ?? 0;

            var newPlan = new BudgetPlan
            {
                Year = sourcePlan.Year,
                VersionNo = maxVersion + 1,
                CreatedAt = now,
                UpdatedAt = now,
                IsDeleted = false
            };
            _context.BudgetPlans.Add(newPlan);
            await _context.SaveChangesAsync();

            var clonedLines = sourcePlan.Lines
                .Where(l => !l.IsDeleted)
                .Select(l => new BudgetLine
                {
                    BudgetPlanId = newPlan.Id,
                    CategoryId = l.CategoryId,
                    Month = l.Month,
                    PlannedAmount = l.PlannedAmount,
                    CreatedAt = now,
                    UpdatedAt = now,
                    IsDeleted = false
                })
                .ToList();

            _context.BudgetLines.AddRange(clonedLines);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Bütçe versiyonu oluşturuldu: V{newPlan.VersionNo}";
            return RedirectToAction(nameof(Edit), new { id = newPlan.Id });
        }

        public async Task<IActionResult> Report(int year)
        {
            var plan = await _context.BudgetPlans
                .Include(b => b.Lines)
                .Where(b => b.Year == year && !b.IsDeleted)
                .OrderByDescending(b => b.VersionNo)
                .FirstOrDefaultAsync();

            var monthLabels = Enumerable.Range(1, 12).Select(m => $"{m:00}.{year}").ToList();
            var plannedMonthly = new decimal[12];
            var actualMonthly = new decimal[12];

            if (plan is not null)
            {
                foreach (var line in plan.Lines.Where(l => !l.IsDeleted))
                {
                    plannedMonthly[line.Month - 1] += line.PlannedAmount;
                }
            }

            var actuals = await _context.CostItems
                .Include(c => c.Category)
                .Where(c => !c.IsDeleted && c.Date.Year == year)
                .ToListAsync();

            foreach (var item in actuals)
            {
                actualMonthly[item.Date.Month - 1] += item.Amount;
            }

            var categories = await _context.Categories.Where(c => !c.IsDeleted).OrderBy(c => c.Name).ToListAsync();
            var rows = new List<BudgetCategoryReportRow>();
            var monthlySeries = new List<CategoryMonthlySeries>();
            foreach (var category in categories)
            {
                var plannedTotal = plan?.Lines.Where(l => l.CategoryId == category.Id && !l.IsDeleted).Sum(l => l.PlannedAmount) ?? 0m;
                var actualTotal = actuals.Where(a => a.CategoryId == category.Id).Sum(a => a.Amount);
                rows.Add(new BudgetCategoryReportRow
                {
                    Name = category.Name ?? "-",
                    PlannedTotal = plannedTotal,
                    ActualTotal = actualTotal
                });

                var plannedMonthlyByCategory = new decimal[12];
                if (plan is not null)
                {
                    foreach (var line in plan.Lines.Where(l => l.CategoryId == category.Id && !l.IsDeleted))
                    {
                        plannedMonthlyByCategory[line.Month - 1] += line.PlannedAmount;
                    }
                }

                var actualMonthlyByCategory = new decimal[12];
                foreach (var item in actuals.Where(a => a.CategoryId == category.Id))
                {
                    actualMonthlyByCategory[item.Date.Month - 1] += item.Amount;
                }

                monthlySeries.Add(new CategoryMonthlySeries
                {
                    Name = category.Name ?? "-",
                    Planned = plannedMonthlyByCategory.ToList(),
                    Actual = actualMonthlyByCategory.ToList()
                });
            }

            var model = new BudgetReportViewModel
            {
                Year = year,
                MonthLabels = monthLabels,
                PlannedMonthlyTotals = plannedMonthly.ToList(),
                ActualMonthlyTotals = actualMonthly.ToList(),
                CategoryRows = rows.OrderByDescending(r => r.Variance).ToList(),
                CategoryMonthlySeries = monthlySeries
            };

            return View(model);
        }
    }
}



