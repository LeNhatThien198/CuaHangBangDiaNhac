using Microsoft.AspNetCore.SignalR;
using CuaHangBangDiaNhac.Models;
using CuaHangBangDiaNhac.Services.Business.Interfaces;

namespace CuaHangBangDiaNhac.Hubs
{
    public class TicketHub : Hub
    {
        private readonly IModeratorTicketService _moderatorService;
        private readonly IUserSupportTicketService _supportService;

        public TicketHub(IModeratorTicketService moderatorService, IUserSupportTicketService supportService)
        {
            _moderatorService = moderatorService;
            _supportService = supportService;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Clients.All.SendAsync("UserConnected", Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
            await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);
        }

        public async Task BroadcastModeratorTicketStatusChanged(int ticketId, string status)
        {
            var ticket = await _moderatorService.GetTicketByIdAsync(ticketId);
            await Clients.All.SendAsync("ModeratorTicketStatusChanged", new { ticketId, status, ticket });
        }

        public async Task BroadcastSupportTicketStatusChanged(int ticketId, string status)
        {
            var ticket = await _supportService.GetTicketByIdAsync(ticketId);
            await Clients.All.SendAsync("SupportTicketStatusChanged", new { ticketId, status, ticket });
        }

        public async Task NotifyNewModeratorTicket(ModeratorTicket ticket)
        {
            await Clients.All.SendAsync("NewModeratorTicketAdded", ticket);
        }

        public async Task NotifyNewSupportTicket(UserSupportTicket ticket)
        {
            await Clients.All.SendAsync("NewSupportTicketAdded", ticket);
        }
    }
}
