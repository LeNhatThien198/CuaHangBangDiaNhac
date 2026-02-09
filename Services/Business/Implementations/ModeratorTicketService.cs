using CuaHangBangDiaNhac.Models;
using CuaHangBangDiaNhac.Repositories.Interfaces;
using CuaHangBangDiaNhac.Services.Business.Interfaces;
using System.Threading.Tasks;

namespace CuaHangBangDiaNhac.Services.Business.Implementations
{
    public class ModeratorTicketService : IModeratorTicketService
    {
        private readonly IModeratorTicketRepository _ticketRepo;
        private readonly IAuditLogService _auditService;

        public ModeratorTicketService(IModeratorTicketRepository ticketRepo, IAuditLogService auditService)
        {
            _ticketRepo = ticketRepo;
            _auditService = auditService;
        }

        public async Task<ModeratorTicket> GetTicketByIdAsync(int id)
        {
            return await _ticketRepo.GetByIdAsync(id);
        }

        public async Task<List<ModeratorTicket>> GetPendingTicketsAsync(int page = 1, int pageSize = 20)
        {
            return await _ticketRepo.GetPendingAsync(page, pageSize);
        }

        public async Task<ModeratorTicket> CreateTicketAsync(ModeratorTicket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException(nameof(ticket));

            if (ticket.ProductId <= 0)
                throw new ArgumentException("ID sản phẩm không hợp lệ", nameof(ticket.ProductId));

            var result = await _ticketRepo.CreateAsync(ticket);

            await _auditService.LogActionAsync("CREATE", "ModeratorTicket", result.Id, "system",
                null, $"Tạo phiếu review cho sản phẩm {result.ProductId}");

            return result;
        }

        public async Task<ModeratorTicket> ApproveTicketAsync(int ticketId, string moderatorId)
        {
            if (string.IsNullOrWhiteSpace(moderatorId))
                throw new ArgumentException("ID người kiểm duyệt không được để trống", nameof(moderatorId));

            var ticket = await _ticketRepo.GetByIdAsync(ticketId);
            if (ticket == null)
                throw new InvalidOperationException("Phiếu review không tồn tại");

            ticket.Status = TicketStatus.Approved;
            ticket.ModeratorId = moderatorId;
            ticket.ResolvedAt = DateTime.UtcNow;

            await _ticketRepo.UpdateAsync(ticket);

            await _auditService.LogActionAsync("APPROVE", "ModeratorTicket", ticketId, moderatorId,
                TicketStatus.Pending.ToString(), TicketStatus.Approved.ToString());

            return ticket;
        }

        public async Task<ModeratorTicket> RejectTicketAsync(int ticketId, string moderatorId, string reason)
        {
            if (string.IsNullOrWhiteSpace(moderatorId))
                throw new ArgumentException("ID người kiểm duyệt không được để trống", nameof(moderatorId));

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Lý do từ chối không được để trống", nameof(reason));

            var ticket = await _ticketRepo.GetByIdAsync(ticketId);
            if (ticket == null)
                throw new InvalidOperationException("Phiếu review không tồn tại");

            ticket.Status = TicketStatus.Rejected;
            ticket.ModeratorId = moderatorId;
            ticket.Reason = reason;
            ticket.ResolvedAt = DateTime.UtcNow;

            await _ticketRepo.UpdateAsync(ticket);

            await _auditService.LogActionAsync("REJECT", "ModeratorTicket", ticketId, moderatorId,
                TicketStatus.Pending.ToString(), $"{TicketStatus.Rejected} - {reason}");

            return ticket;
        }

        public async Task DeleteTicketAsync(int id)
        {
            var ticket = await _ticketRepo.GetByIdAsync(id);
            if (ticket == null)
                throw new InvalidOperationException("Phiếu review không tồn tại");

            await _ticketRepo.DeleteAsync(id);

            await _auditService.LogActionAsync("DELETE", "ModeratorTicket", id, "system",
                ticket.Status.ToString(), null);
        }
    }
}
