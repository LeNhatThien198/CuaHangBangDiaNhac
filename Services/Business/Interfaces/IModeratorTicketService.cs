using CuaHangBangDiaNhac.Models;

namespace CuaHangBangDiaNhac.Services.Business.Interfaces
{
    public interface IModeratorTicketService
    {
        Task<ModeratorTicket> GetTicketByIdAsync(int id);
        Task<List<ModeratorTicket>> GetPendingTicketsAsync(int page = 1, int pageSize = 20);
        Task<ModeratorTicket> CreateTicketAsync(ModeratorTicket ticket);
        Task<ModeratorTicket> ApproveTicketAsync(int ticketId, string moderatorId);
        Task<ModeratorTicket> RejectTicketAsync(int ticketId, string moderatorId, string reason);
        Task DeleteTicketAsync(int id);
    }
}
