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
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Type)
                .ThenBy(x => x.Name)
                .ToListAsync();

            return View(items);
        }

        public IActionResult Create()
        {
            return View(new Counterparty());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Type,TaxNumber,Phone,Email,Address,Note")] Counterparty model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

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

            var item = await _context.Counterparties
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (item is null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Type,TaxNumber,Phone,Email,Address,Note")] Counterparty input)
        {
            if (id != input.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(input);
            }

            var item = await _context.Counterparties
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (item is null)
            {
                return NotFound();
            }

            item.Name = input.Name;
            item.Type = input.Type;
            item.TaxNumber = input.TaxNumber;
            item.Phone = input.Phone;
            item.Email = input.Email;
            item.Address = input.Address;
            item.Note = input.Note;
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
