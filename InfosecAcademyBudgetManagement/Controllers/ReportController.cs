using InfosecAcademyBudgetManagement.Data;
using InfosecAcademyBudgetManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfosecAcademyBudgetManagement.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? year)
        {
            var selectedYear = year ?? DateTime.UtcNow.Year;
            var expenses = await _context.CostItems
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Date.Year == selectedYear)
                .ToListAsync();

            var incomes = await _context.IncomeItems
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Date.Year == selectedYear)
                .ToListAsync();

            var monthLabels = Enumerable.Range(1, 12).Select(x => $"{x:00}.{selectedYear}").ToList();
            var incomeMonthly = new List<decimal>();
            var expenseMonthly = new List<decimal>();
            var netMonthly = new List<decimal>();

            for (var month = 1; month <= 12; month++)
            {
                var income = incomes.Where(x => x.Date.Month == month).Sum(x => x.Amount);
                var expense = expenses.Where(x => x.Date.Month == month).Sum(x => x.Amount);
                incomeMonthly.Add(income);
                expenseMonthly.Add(expense);
                netMonthly.Add(income - expense);
            }

            var model = new AdvancedReportViewModel
            {
                Year = selectedYear,
                MonthLabels = monthLabels,
                IncomeMonthly = incomeMonthly,
                ExpenseMonthly = expenseMonthly,
                NetMonthly = netMonthly,
                TotalIncome = incomes.Sum(x => x.Amount),
                TotalExpense = expenses.Sum(x => x.Amount)
            };

            return View(model);
        }
    }
}
