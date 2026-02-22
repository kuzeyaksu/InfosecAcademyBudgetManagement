using InfosecAcademyBudgetManagement.Data;
using InfosecAcademyBudgetManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfosecAcademyBudgetManagement.Controllers
{
    [Authorize(Roles = "Yönetici")]
    public class AuditLogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuditLogController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var logs = await _context.AuditLogs
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Take(500)
                .Select(x => new AuditLogListItemViewModel
                {
                    CreatedAt = x.CreatedAt,
                    UserName = x.UserName,
                    Method = x.Method,
                    Path = x.Path,
                    StatusCode = x.StatusCode,
                    IpAddress = x.IpAddress
                })
                .ToListAsync();
            return View(logs);
        }
    }
}
