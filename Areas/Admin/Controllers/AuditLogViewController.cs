using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CuaHangBangDiaNhac.Services.Business.Interfaces;

namespace CuaHangBangDiaNhac.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/auditlog")]
    [Authorize(Roles = "Admin,Staff")]
    public class AuditLogViewController : Controller
    {
        private readonly IAuditLogService _auditService;

        public AuditLogViewController(IAuditLogService auditService)
        {
            _auditService = auditService;
        }

        [HttpGet("")]
        [HttpGet("index")]
        public async Task<IActionResult> Index(int page = 1)
        {
            var logs = await _auditService.GetLogsAsync(page, 50);
            return View(logs);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> UserLogs(string userId, int page = 1)
        {
            var logs = await _auditService.GetUserLogsAsync(userId, page, 50);
            return View("Index", logs);
        }
    }
}
