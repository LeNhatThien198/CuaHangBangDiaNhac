using CuaHangBangDiaNhac.Data;
using CuaHangBangDiaNhac.Models;
using CuaHangBangDiaNhac.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CuaHangBangDiaNhac.Repositories.Implementations
{
    public class UserSupportTicketRepository : IUserSupportTicketRepository
    {
        private readonly ApplicationDbContext _context;

        public UserSupportTicketRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserSupportTicket> GetByIdAsync(int id)
        {
            return await _context.UserSupportTickets
                .Include(ust => ust.User)
                .Include(ust => ust.AssignedTo)
                .AsNoTracking()
                .FirstOrDefaultAsync(ust => ust.Id == id);
        }

        public async Task<List<UserSupportTicket>> GetByUserAsync(string userId, int page = 1, int pageSize = 20)
        {
            return await _context.UserSupportTickets
                .Include(ust => ust.User)
                .Include(ust => ust.AssignedTo)
                .Where(ust => ust.UserId == userId)
                .AsNoTracking()
                .OrderByDescending(ust => ust.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<UserSupportTicket>> GetOpenAsync(int page = 1, int pageSize = 20)
        {
            return await _context.UserSupportTickets
                .Include(ust => ust.User)
                .Include(ust => ust.AssignedTo)
                .Where(ust => ust.Status == SupportStatus.Open || ust.Status == SupportStatus.InProgress)
                .AsNoTracking()
                .OrderByDescending(ust => ust.Priority)
                .ThenBy(ust => ust.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<UserSupportTicket>> GetByStatusAsync(SupportStatus status, int page = 1, int pageSize = 20)
        {
            return await _context.UserSupportTickets
                .Include(ust => ust.User)
                .Include(ust => ust.AssignedTo)
                .Where(ust => ust.Status == status)
                .AsNoTracking()
                .OrderByDescending(ust => ust.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<UserSupportTicket> CreateAsync(UserSupportTicket ticket)
        {
            _context.UserSupportTickets.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task UpdateAsync(UserSupportTicket ticket)
        {
            _context.UserSupportTickets.Update(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var ticket = await _context.UserSupportTickets.FindAsync(id);
            if (ticket != null)
            {
                _context.UserSupportTickets.Remove(ticket);
                await _context.SaveChangesAsync();
            }
        }
    }
}
