using CuaHangBangDiaNhac.Data;
using CuaHangBangDiaNhac.Models;
using CuaHangBangDiaNhac.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CuaHangBangDiaNhac.Repositories.Implementations
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly ApplicationDbContext _context;

        public AuditLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AuditLog> CreateAsync(AuditLog log)
        {
            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
            return log;
        }

        public async Task<List<AuditLog>> GetByEntityAsync(string entityName, int? entityId)
        {
            var query = _context.AuditLogs
                .AsNoTracking()
                .Where(al => al.EntityName == entityName);

            if (entityId.HasValue)
            {
                query = query.Where(al => al.EntityId == entityId.Value);
            }

            return await query
                .OrderByDescending(al => al.Timestamp)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetByUserAsync(string userId)
        {
            return await _context.AuditLogs
                .AsNoTracking()
                .Where(al => al.UserId == userId)
                .OrderByDescending(al => al.Timestamp)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            return await _context.AuditLogs
                .AsNoTracking()
                .Where(al => al.Timestamp >= from && al.Timestamp <= to)
                .OrderByDescending(al => al.Timestamp)
                .ToListAsync();
        }
    }
}
