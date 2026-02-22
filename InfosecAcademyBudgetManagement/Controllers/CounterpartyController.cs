using InfosecAcademyBudgetManagement.Data;
using InfosecAcademyBudgetManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfosecAcademyBudgetManagement.Controllers
{
    [Authorize]
    public class CounterpartyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CounterpartyController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _context.Counterparties
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Type)
                .ThenBy(x => x.Name)
                .Select(x => new CounterpartyListItemViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = x.Type,
                    TaxNumber = x.TaxNumber,
                    Phone = x.Phone,
                    Email = x.Email
                })
                .ToListAsync();

            return View(items);
        }

        public IActionResult Create()
        {
            return View(new CounterpartyFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CounterpartyFormViewModel form)
        {
            if (!ModelState.IsValid)
            {
                return View(form);
            }

            var model = new Counterparty
            {
                Name = form.Name,
                Type = form.Type,
                TaxNumber = form.TaxNumber,
                Phone = form.Phone,
                Email = form.Email,
                Address = form.Address,
                Note = form.Note
            };
            var now = DateTime.UtcNow;
            model.CreatedAt = now;
            model.UpdatedAt = now;

            _context.Counterparties.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var item = await _context.Counterparties
                .AsNoTracking()
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new CounterpartyDetailViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = x.Type,
                    TaxNumber = x.TaxNumber,
                    Phone = x.Phone,
                    Email = x.Email,
                    Address = x.Address,
                    Note = x.Note
                })
                .FirstOrDefaultAsync();

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

            var item = await _context.Counterparties
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (item is null)
            {
                return NotFound();
            }

            var form = new CounterpartyFormViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Type = item.Type,
                TaxNumber = item.TaxNumber,
                Phone = item.Phone,
                Email = item.Email,
                Address = item.Address,
                Note = item.Note
            };

            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CounterpartyFormViewModel form)
        {
            if (!form.Id.HasValue)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(form);
            }

            var item = await _context.Counterparties
                .FirstOrDefaultAsync(x => x.Id == form.Id.Value && !x.IsDeleted);

            if (item is null)
            {
                return NotFound();
            }

            item.Name = form.Name;
            item.Type = form.Type;
            item.TaxNumber = form.TaxNumber;
            item.Phone = form.Phone;
            item.Email = form.Email;
            item.Address = form.Address;
            item.Note = form.Note;
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

            var item = await _context.Counterparties
                .AsNoTracking()
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new CounterpartyDetailViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = x.Type,
                    TaxNumber = x.TaxNumber,
                    Phone = x.Phone,
                    Email = x.Email,
                    Address = x.Address,
                    Note = x.Note
                })
                .FirstOrDefaultAsync();

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
            var item = await _context.Counterparties
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
