using CuaHangBangDiaNhac.Data;
using CuaHangBangDiaNhac.Models;
using CuaHangBangDiaNhac.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CuaHangBangDiaNhac.Repositories.Implementations
{
    public class ModeratorTicketRepository : IModeratorTicketRepository
    {
        private readonly ApplicationDbContext _context;

        public ModeratorTicketRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ModeratorTicket> GetByIdAsync(int id)
        {
            return await _context.ModeratorTickets
                .Include(mt => mt.Product)
                .Include(mt => mt.Moderator)
                .AsNoTracking()
                .FirstOrDefaultAsync(mt => mt.Id == id);
        }

        public async Task<List<ModeratorTicket>> GetPendingAsync(int page = 1, int pageSize = 20)
        {
            return await _context.ModeratorTickets
                .Include(mt => mt.Product)
                .Include(mt => mt.Moderator)
                .Where(mt => mt.Status == TicketStatus.Pending)
                .AsNoTracking()
                .OrderBy(mt => mt.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<ModeratorTicket>> GetByStatusAsync(TicketStatus status, int page = 1, int pageSize = 20)
        {
            return await _context.ModeratorTickets
                .Include(mt => mt.Product)
                .Include(mt => mt.Moderator)
                .Where(mt => mt.Status == status)
                .AsNoTracking()
                .OrderByDescending(mt => mt.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<ModeratorTicket> CreateAsync(ModeratorTicket ticket)
        {
            _context.ModeratorTickets.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task UpdateAsync(ModeratorTicket ticket)
        {
            _context.ModeratorTickets.Update(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var ticket = await _context.ModeratorTickets.FindAsync(id);
            if (ticket != null)
            {
                _context.ModeratorTickets.Remove(ticket);
                await _context.SaveChangesAsync();
            }
        }
    }
}
