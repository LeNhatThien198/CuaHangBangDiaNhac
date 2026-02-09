using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CuaHangBangDiaNhac.Services.Business.Interfaces;

namespace CuaHangBangDiaNhac.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/moderatorticket")]
    [Authorize(Roles = "Admin,Staff")]
    public class ModeratorTicketViewController : Controller
    {
        private readonly IModeratorTicketService _ticketService;

        public ModeratorTicketViewController(IModeratorTicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet("")]
        [HttpGet("index")]
        public async Task<IActionResult> Index(int page = 1)
        {
            var tickets = await _ticketService.GetPendingTicketsAsync(page);
            return View(tickets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null)
                return NotFound();
            return View(ticket);
        }
    }
}
