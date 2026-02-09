using CuaHangBangDiaNhac.Models;

namespace CuaHangBangDiaNhac.Services.Business.Interfaces
{
    public interface IUserSupportTicketService
    {
        Task<UserSupportTicket> GetTicketByIdAsync(int id);
        Task<List<UserSupportTicket>> GetUserTicketsAsync(string userId, int page = 1, int pageSize = 20);
        Task<List<UserSupportTicket>> GetOpenTicketsAsync(int page = 1, int pageSize = 20);
        Task<UserSupportTicket> CreateTicketAsync(UserSupportTicket ticket);
        Task<UserSupportTicket> AssignTicketAsync(int ticketId, string staffId);
        Task<UserSupportTicket> ResolveTicketAsync(int ticketId);
        Task<UserSupportTicket> CloseTicketAsync(int ticketId);
        Task DeleteTicketAsync(int id);
    }
}
