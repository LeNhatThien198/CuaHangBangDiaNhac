using Microsoft.AspNetCore.SignalR;

namespace CuaHangBangDiaNhac.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotificationToUser(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }

        public async Task SendNotificationToAdmins(string message, object data = null)
        {
            await Clients.Group("Admins").SendAsync("ReceiveAdminNotification", new { message, data });
        }

        public async Task JoinAdminGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        }

        public async Task LeaveAdminGroup()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Admins");
        }
    }
}
