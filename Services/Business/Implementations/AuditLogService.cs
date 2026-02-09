using CuaHangBangDiaNhac.Models;
using CuaHangBangDiaNhac.Repositories.Interfaces;
using CuaHangBangDiaNhac.Services.Business.Interfaces;
using System.Threading.Tasks;

namespace CuaHangBangDiaNhac.Services.Business.Implementations
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditRepo;

        public AuditLogService(IAuditLogRepository auditRepo)
        {
            _auditRepo = auditRepo;
        }

        public async Task<AuditLog> LogActionAsync(string action, string entityName, int? entityId, string userId, string? oldValue = null, string? newValue = null)
        {
            if (string.IsNullOrWhiteSpace(action))
                throw new ArgumentException("Hành động không được để trống", nameof(action));

            if (string.IsNullOrWhiteSpace(entityName))
                throw new ArgumentException("Tên thực thể không được để trống", nameof(entityName));

            var auditLog = new AuditLog
            {
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                UserId = userId,
                OldValue = oldValue,
                NewValue = newValue,
                Timestamp = DateTime.UtcNow
            };

            return await _auditRepo.CreateAsync(auditLog);
        }

        public async Task<List<AuditLog>> GetLogsAsync(int page = 1, int pageSize = 50)
        {
            var from = DateTime.UtcNow.AddDays(-30);
            var to = DateTime.UtcNow;

            var logs = await _auditRepo.GetByDateRangeAsync(from, to);
            return logs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public async Task<List<AuditLog>> GetUserLogsAsync(string userId, int page = 1, int pageSize = 50)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID không được để trống", nameof(userId));

            var logs = await _auditRepo.GetByUserAsync(userId);
            return logs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
    }
}
