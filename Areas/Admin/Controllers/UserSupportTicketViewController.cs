using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CuaHangBangDiaNhac.Services.Business.Interfaces;

namespace CuaHangBangDiaNhac.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/usersupportticket")]
    [Authorize(Roles = "Admin,Staff")]
    public class UserSupportTicketViewController : Controller
    {
        private readonly IUserSupportTicketService _ticketService;

        public UserSupportTicketViewController(IUserSupportTicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet("")]
        [HttpGet("index")]
        public async Task<IActionResult> Index(int page = 1)
        {
            var tickets = await _ticketService.GetOpenTicketsAsync(page);
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
