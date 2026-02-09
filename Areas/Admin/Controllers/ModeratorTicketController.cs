using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CuaHangBangDiaNhac.Services.Business.Interfaces;
using CuaHangBangDiaNhac.Models;

namespace CuaHangBangDiaNhac.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ModeratorTicketController : Controller
    {
        private readonly IModeratorTicketService _ticketService;
        private readonly IAuditLogService _auditService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ModeratorTicketController(IModeratorTicketService ticketService, IAuditLogService auditService, IHttpContextAccessor httpContextAccessor)
        {
            _ticketService = ticketService;
            _auditService = auditService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingAsync(int page = 1)
        {
            try
            {
                var tickets = await _ticketService.GetPendingTicketsAsync(page);
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

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveAsync(int id)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value ?? "System";
                var ticket = await _ticketService.ApproveTicketAsync(id, userId);
                await _auditService.LogActionAsync("APPROVE_TICKET", "ModeratorTicket", id, userId, null, $"Approved ticket {id}");
                
                return Json(new { success = true, message = "Ticket approved successfully", data = ticket });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectAsync(int id, [FromBody] dynamic request)
        {
            try
            {
                string reason = request?.reason ?? "";
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value ?? "System";
                var ticket = await _ticketService.RejectTicketAsync(id, userId, reason);
                await _auditService.LogActionAsync("REJECT_TICKET", "ModeratorTicket", id, userId, null, $"Rejected: {reason}");
                
                return Json(new { success = true, message = "Ticket rejected successfully", data = ticket });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
