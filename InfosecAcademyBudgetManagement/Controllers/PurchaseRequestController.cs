using InfosecAcademyBudgetManagement.Data;
using InfosecAcademyBudgetManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfosecAcademyBudgetManagement.Controllers
{
    [Authorize]
    public class PurchaseRequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchaseRequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _context.PurchaseRequests
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new PurchaseRequestListItemViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Amount = x.Amount,
                    RequestDate = x.RequestDate,
                    Status = x.Status,
                    RequestedBy = x.RequestedByUserId,
                    ApprovedBy = x.ApprovedByUserId
                })
                .ToListAsync();
            return View(items);
        }

        public IActionResult Create()
        {
            return View(new PurchaseRequestFormViewModel { RequestDate = DateTime.UtcNow });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseRequestFormViewModel form)
        {
            if (!ModelState.IsValid)
            {
                return View(form);
            }

            var now = DateTime.UtcNow;
            var entity = new PurchaseRequest
            {
                Title = form.Title,
                Description = form.Description,
                Amount = form.Amount,
                RequestDate = form.RequestDate,
                CreatedAt = now,
                UpdatedAt = now,
                Status = "Beklemede",
                RequestedByUserId = User.Identity?.Name
            };

            _context.PurchaseRequests.Add(entity);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Satın alma talebi oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.PurchaseRequests
                .AsNoTracking()
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new PurchaseRequestDetailViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    Amount = x.Amount,
                    RequestDate = x.RequestDate,
                    Status = x.Status,
                    RequestedBy = x.RequestedByUserId,
                    ApprovedBy = x.ApprovedByUserId,
                    ApprovedAt = x.ApprovedAt,
                    RejectionReason = x.RejectionReason
                })
                .FirstOrDefaultAsync();
            return item is null ? NotFound() : View(item);
        }

        [HttpPost]
        [Authorize(Roles = "Yönetici,Müdür")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var item = await _context.PurchaseRequests.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (item is null)
            {
                return NotFound();
            }

            item.Status = "Onaylandı";
            item.ApprovedByUserId = User.Identity?.Name;
            item.ApprovedAt = DateTime.UtcNow;
            item.RejectionReason = null;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Talep onaylandı.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Yönetici,Müdür")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string? reason)
        {
            var item = await _context.PurchaseRequests.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (item is null)
            {
                return NotFound();
            }

            item.Status = "Reddedildi";
            item.ApprovedByUserId = User.Identity?.Name;
            item.ApprovedAt = DateTime.UtcNow;
            item.RejectionReason = string.IsNullOrWhiteSpace(reason) ? "Gerekçe belirtilmedi." : reason.Trim();
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Talep reddedildi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
