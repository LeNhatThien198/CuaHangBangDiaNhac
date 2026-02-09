using Microsoft.AspNetCore.SignalR;
using CuaHangBangDiaNhac.Models;
using CuaHangBangDiaNhac.Services.Business.Interfaces;

namespace CuaHangBangDiaNhac.Hubs
{
    public class AuditLogHub : Hub
    {
        private readonly IAuditLogService _auditService;

        public AuditLogHub(IAuditLogService auditService)
        {
            _auditService = auditService;
        }

        public async Task BroadcastAuditLog(AuditLog log)
        {
            await Clients.All.SendAsync("NewAuditLogAdded", log);
        }

        public async Task RequestAuditLogs(DateTime from, DateTime to)
        {
            var logs = await _auditService.GetLogsAsync(1, 100);
            await Clients.Caller.SendAsync("AuditLogsRetrieved", logs);
        }
    }
}
