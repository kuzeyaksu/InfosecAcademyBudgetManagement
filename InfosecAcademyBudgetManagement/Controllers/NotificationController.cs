using InfosecAcademyBudgetManagement.Data;
using InfosecAcademyBudgetManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfosecAcademyBudgetManagement.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var now = DateTime.UtcNow;
            var currentYear = now.Year;
            var currentMonth = now.Month;

            var model = new NotificationViewModel();

            var yearPlan = await _context.BudgetPlans
                .Include(b => b.Lines)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Year == currentYear && !b.IsDeleted);

            var yearPlannedExpense = yearPlan?.Lines.Where(x => !x.IsDeleted).Sum(x => x.PlannedAmount) ?? 0m;
            var monthPlannedExpense = yearPlan?.Lines.Where(x => !x.IsDeleted && x.Month == currentMonth).Sum(x => x.PlannedAmount) ?? 0m;

            var yearActualExpense = await _context.CostItems
                .Where(x => !x.IsDeleted && x.Date.Year == currentYear)
                .SumAsync(x => x.Amount);

            var monthActualExpense = await _context.CostItems
                .Where(x => !x.IsDeleted && x.Date.Year == currentYear && x.Date.Month == currentMonth)
                .SumAsync(x => x.Amount);

            var monthActualIncome = await _context.IncomeItems
                .Where(x => !x.IsDeleted && x.Date.Year == currentYear && x.Date.Month == currentMonth)
                .SumAsync(x => x.Amount);

            if (yearPlan is null)
            {
                model.Items.Add(new NotificationItem
                {
                    Level = NotificationLevel.Warning,
                    Title = "Bütçe Planı Eksik",
                    Message = $"{currentYear} yılı için bütçe planı bulunamadı."
                });
            }
            else
            {
                if (yearPlannedExpense > 0 && yearActualExpense > yearPlannedExpense)
                {
                    model.Items.Add(new NotificationItem
                    {
                        Level = NotificationLevel.Critical,
                        Title = "Yıllık Bütçe Aşıldı",
                        Message = $"Gerçekleşen gider {yearActualExpense:N2} TL, yıllık plan {yearPlannedExpense:N2} TL."
                    });
                }

                if (monthPlannedExpense > 0 && monthActualExpense > monthPlannedExpense)
                {
                    model.Items.Add(new NotificationItem
                    {
                        Level = NotificationLevel.Warning,
                        Title = "Aylık Bütçe Aşıldı",
                        Message = $"{currentMonth:00}/{currentYear} döneminde gider {monthActualExpense:N2} TL, plan {monthPlannedExpense:N2} TL."
                    });
                }
            }

            if (monthActualExpense > monthActualIncome)
            {
                model.Items.Add(new NotificationItem
                {
                    Level = NotificationLevel.Warning,
                    Title = "Aylık Net Nakit Negatif",
                    Message = $"{currentMonth:00}/{currentYear} için gelir {monthActualIncome:N2} TL, gider {monthActualExpense:N2} TL."
                });
            }

            var missingEmailSettings = !await _context.EmailAccountSettings.AsNoTracking().AnyAsync();
            if (missingEmailSettings)
            {
                model.Items.Add(new NotificationItem
                {
                    Level = NotificationLevel.Info,
                    Title = "E-posta Ayarları Eksik",
                    Message = "Sistem bildirimlerini kullanmak için SMTP ayarlarını yapılandırın."
                });
            }

            var last30DaysHighCosts = await _context.CostItems
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Date >= now.AddDays(-30))
                .OrderByDescending(x => x.Amount)
                .Take(1)
                .Select(x => new { x.Amount, x.Name, x.Date })
                .FirstOrDefaultAsync();

            if (last30DaysHighCosts is not null && last30DaysHighCosts.Amount >= 50000m)
            {
                model.Items.Add(new NotificationItem
                {
                    Level = NotificationLevel.Info,
                    Title = "Yüksek Tutar Uyarısı",
                    Message = $"Son 30 günde {last30DaysHighCosts.Date:dd.MM.yyyy} tarihli {last30DaysHighCosts.Name} kaydı {last30DaysHighCosts.Amount:N2} TL."
                });
            }

            if (model.Items.Count == 0)
            {
                model.Items.Add(new NotificationItem
                {
                    Level = NotificationLevel.Info,
                    Title = "Bildirim Yok",
                    Message = "Şu an kritik veya uyarı gerektiren bir durum bulunmuyor."
                });
            }

            return View(model);
        }
    }
}
