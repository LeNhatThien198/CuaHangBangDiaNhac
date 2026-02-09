using CuaHangBangDiaNhac.Models;

namespace CuaHangBangDiaNhac.Repositories.Interfaces
{
    public interface IUserSupportTicketRepository
    {
        Task<UserSupportTicket> GetByIdAsync(int id);
        Task<List<UserSupportTicket>> GetByUserAsync(string userId, int page = 1, int pageSize = 20);
        Task<List<UserSupportTicket>> GetOpenAsync(int page = 1, int pageSize = 20);
        Task<List<UserSupportTicket>> GetByStatusAsync(SupportStatus status, int page = 1, int pageSize = 20);
        Task<UserSupportTicket> CreateAsync(UserSupportTicket ticket);
        Task UpdateAsync(UserSupportTicket ticket);
        Task DeleteAsync(int id);
    }
}
