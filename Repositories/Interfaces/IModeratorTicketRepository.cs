using CuaHangBangDiaNhac.Models;

namespace CuaHangBangDiaNhac.Repositories.Interfaces
{
    public interface IModeratorTicketRepository
    {
        Task<ModeratorTicket> GetByIdAsync(int id);
        Task<List<ModeratorTicket>> GetPendingAsync(int page = 1, int pageSize = 20);
        Task<List<ModeratorTicket>> GetByStatusAsync(TicketStatus status, int page = 1, int pageSize = 20);
        Task<ModeratorTicket> CreateAsync(ModeratorTicket ticket);
        Task UpdateAsync(ModeratorTicket ticket);
        Task DeleteAsync(int id);
    }
}
