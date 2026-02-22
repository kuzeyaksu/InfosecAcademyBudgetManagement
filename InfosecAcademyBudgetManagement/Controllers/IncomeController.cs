using InfosecAcademyBudgetManagement.Data;
using InfosecAcademyBudgetManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfosecAcademyBudgetManagement.Controllers
{
    [Authorize]
    public class IncomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IncomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _context.IncomeItems
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.Date)
                .ToListAsync();

            return View(items);
        }

        public IActionResult Create()
        {
            return View(new IncomeItem { Date = DateTime.UtcNow });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Amount,Date,Source,Counterparty,Description")] IncomeItem income)
        {
            if (!ModelState.IsValid)
            {
                return View(income);
            }

            var now = DateTime.UtcNow;
            income.CreatedAt = now;
            income.UpdatedAt = now;
            income.Date = income.Date == default ? now : income.Date;

            _context.IncomeItems.Add(income);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var item = await _context.IncomeItems
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (item is null)
            {
                return NotFound();
            }

            return View(item);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var item = await _context.IncomeItems
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (item is null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Amount,Date,Source,Counterparty,Description")] IncomeItem input)
        {
            if (id != input.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(input);
            }

            var item = await _context.IncomeItems
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (item is null)
            {
                return NotFound();
            }

            item.Name = input.Name;
            item.Amount = input.Amount;
            item.Date = input.Date == default ? DateTime.UtcNow : input.Date;
            item.Source = input.Source;
            item.Counterparty = input.Counterparty;
            item.Description = input.Description;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var item = await _context.IncomeItems
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (item is null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.IncomeItems
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (item is null)
            {
                return RedirectToAction(nameof(Index));
            }

            var now = DateTime.UtcNow;
            item.IsDeleted = true;
            item.DeletedAt = now;
            item.UpdatedAt = now;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
