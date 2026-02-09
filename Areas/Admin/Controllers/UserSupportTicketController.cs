using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CuaHangBangDiaNhac.Services.Business.Interfaces;
using CuaHangBangDiaNhac.Models;

namespace CuaHangBangDiaNhac.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UserSupportTicketController : Controller
    {
        private readonly IUserSupportTicketService _ticketService;
        private readonly IAuditLogService _auditService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserSupportTicketController(IUserSupportTicketService ticketService, IAuditLogService auditService, IHttpContextAccessor httpContextAccessor)
        {
            _ticketService = ticketService;
            _auditService = auditService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("open")]
        public async Task<IActionResult> GetOpenAsync(int page = 1)
        {
            try
            {
                var tickets = await _ticketService.GetOpenTicketsAsync(page);
                return Json(new { success = true, data = tickets });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketAsync(int id)
        {
            try
            {
                var ticket = await _ticketService.GetTicketByIdAsync(id);
                if (ticket == null)
                    return NotFound(new { success = false, message = "Ticket not found" });
                return Json(new { success = true, data = ticket });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("{id}/assign")]
        public async Task<IActionResult> AssignAsync(int id)
        {
            try
            {
                var staffId = _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value ?? "System";
                var ticket = await _ticketService.AssignTicketAsync(id, staffId);
                await _auditService.LogActionAsync("ASSIGN_TICKET", "UserSupportTicket", id, staffId, null, $"Assigned to {staffId}");
                
                return Json(new { success = true, message = "Ticket assigned successfully", data = ticket });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("{id}/resolve")]
        public async Task<IActionResult> ResolveAsync(int id)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value ?? "System";
                var ticket = await _ticketService.ResolveTicketAsync(id);
                await _auditService.LogActionAsync("RESOLVE_TICKET", "UserSupportTicket", id, userId);
                
                return Json(new { success = true, message = "Ticket resolved successfully", data = ticket });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("{id}/close")]
        public async Task<IActionResult> CloseAsync(int id)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value ?? "System";
                var ticket = await _ticketService.CloseTicketAsync(id);
                await _auditService.LogActionAsync("CLOSE_TICKET", "UserSupportTicket", id, userId);
                
                return Json(new { success = true, message = "Ticket closed successfully", data = ticket });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
