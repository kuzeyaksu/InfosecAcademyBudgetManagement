using InfosecAcademyBudgetManagement.Data;
using InfosecAcademyBudgetManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InfosecAcademyBudgetManagement.Controllers
{
    [Authorize]
    public class PaymentScheduleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentScheduleController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _context.PaymentScheduleItems
                .AsNoTracking()
                .Include(x => x.Counterparty)
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.IsSettled)
                .ThenBy(x => x.DueDate)
                .Select(x => new PaymentScheduleListItemViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Type = x.Type,
                    Amount = x.Amount,
                    DueDate = x.DueDate,
                    IsSettled = x.IsSettled,
                    CounterpartyName = x.Counterparty != null ? x.Counterparty.Name : "-"
                })
                .ToListAsync();

            return View(items);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateCounterpartiesAsync();
            return View(new PaymentScheduleFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaymentScheduleFormViewModel form)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCounterpartiesAsync(form.CounterpartyId);
                return View(form);
            }

            var now = DateTime.UtcNow;
            var entity = new PaymentScheduleItem
            {
                Title = form.Title,
                Type = form.Type,
                Amount = form.Amount,
                DueDate = form.DueDate,
                CounterpartyId = form.CounterpartyId,
                Note = form.Note,
                CreatedAt = now,
                UpdatedAt = now,
                IsSettled = false
            };

            _context.PaymentScheduleItems.Add(entity);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Takvim kaydı oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleSettled(int id)
        {
            var entity = await _context.PaymentScheduleItems.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (entity is null)
            {
                return NotFound();
            }

            entity.IsSettled = !entity.IsSettled;
            entity.SettledAt = entity.IsSettled ? DateTime.UtcNow : null;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.PaymentScheduleItems.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (entity is null)
            {
                return RedirectToAction(nameof(Index));
            }

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateCounterpartiesAsync(int? selectedId = null)
        {
            var data = await _context.Counterparties
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Name)
                .ToListAsync();
            ViewData["Counterparties"] = new SelectList(data, "Id", "Name", selectedId);
        }
    }
}
