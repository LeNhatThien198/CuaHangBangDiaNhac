using CuaHangBangDiaNhac.Models;

namespace CuaHangBangDiaNhac.Repositories.Interfaces
{
    public interface IAuditLogRepository
    {
        Task<AuditLog> CreateAsync(AuditLog log);
        Task<List<AuditLog>> GetByEntityAsync(string entityName, int? entityId);
        Task<List<AuditLog>> GetByUserAsync(string userId);
        Task<List<AuditLog>> GetByDateRangeAsync(DateTime from, DateTime to);
    }
}
