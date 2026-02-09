using CuaHangBangDiaNhac.Models;

namespace CuaHangBangDiaNhac.Services.Business.Interfaces
{
    public interface IAuditLogService
    {
        Task<AuditLog> LogActionAsync(string action, string entityName, int? entityId, string userId, string? oldValue = null, string? newValue = null);
        Task<List<AuditLog>> GetLogsAsync(int page = 1, int pageSize = 50);
        Task<List<AuditLog>> GetUserLogsAsync(string userId, int page = 1, int pageSize = 50);
    }
}
