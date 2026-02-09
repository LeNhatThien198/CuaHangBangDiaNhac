using CuaHangBangDiaNhac.Models;
using CuaHangBangDiaNhac.Repositories.Interfaces;
using CuaHangBangDiaNhac.Services.Business.Interfaces;
using System.Threading.Tasks;

namespace CuaHangBangDiaNhac.Services.Business.Implementations
{
    public class UserSupportTicketService : IUserSupportTicketService
    {
        private readonly IUserSupportTicketRepository _ticketRepo;
        private readonly IAuditLogService _auditService;

        public UserSupportTicketService(IUserSupportTicketRepository ticketRepo, IAuditLogService auditService)
        {
            _ticketRepo = ticketRepo;
            _auditService = auditService;
        }

        public async Task<UserSupportTicket> GetTicketByIdAsync(int id)
        {
            return await _ticketRepo.GetByIdAsync(id);
        }

        public async Task<List<UserSupportTicket>> GetUserTicketsAsync(string userId, int page = 1, int pageSize = 20)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("ID người dùng không được để trống", nameof(userId));

            return await _ticketRepo.GetByUserAsync(userId, page, pageSize);
        }

        public async Task<List<UserSupportTicket>> GetOpenTicketsAsync(int page = 1, int pageSize = 20)
        {
            return await _ticketRepo.GetOpenAsync(page, pageSize);
        }

        public async Task<UserSupportTicket> CreateTicketAsync(UserSupportTicket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException(nameof(ticket));

            if (string.IsNullOrWhiteSpace(ticket.Title))
                throw new ArgumentException("Tiêu đề không được để trống", nameof(ticket.Title));

            if (string.IsNullOrWhiteSpace(ticket.Description))
                throw new ArgumentException("Mô tả không được để trống", nameof(ticket.Description));

            if (string.IsNullOrWhiteSpace(ticket.UserId))
                throw new ArgumentException("ID người dùng không được để trống", nameof(ticket.UserId));

            var result = await _ticketRepo.CreateAsync(ticket);

            await _auditService.LogActionAsync("CREATE", "UserSupportTicket", result.Id, ticket.UserId,
                null, $"Tạo phiếu hỗ trợ: {result.Title}");

            return result;
        }

        public async Task<UserSupportTicket> AssignTicketAsync(int ticketId, string staffId)
        {
            if (string.IsNullOrWhiteSpace(staffId))
                throw new ArgumentException("ID nhân viên không được để trống", nameof(staffId));

            var ticket = await _ticketRepo.GetByIdAsync(ticketId);
            if (ticket == null)
                throw new InvalidOperationException("Phiếu hỗ trợ không tồn tại");

            var oldAssignee = ticket.AssignedToId ?? "không";
            ticket.AssignedToId = staffId;
            ticket.Status = SupportStatus.InProgress;

            await _ticketRepo.UpdateAsync(ticket);

            await _auditService.LogActionAsync("ASSIGN", "UserSupportTicket", ticketId, staffId,
                oldAssignee, staffId);

            return ticket;
        }

        public async Task<UserSupportTicket> ResolveTicketAsync(int ticketId)
        {
            var ticket = await _ticketRepo.GetByIdAsync(ticketId);
            if (ticket == null)
                throw new InvalidOperationException("Phiếu hỗ trợ không tồn tại");

            ticket.Status = SupportStatus.Resolved;
            ticket.ResolvedAt = DateTime.UtcNow;

            await _ticketRepo.UpdateAsync(ticket);

            await _auditService.LogActionAsync("RESOLVE", "UserSupportTicket", ticketId, ticket.AssignedToId ?? "system",
                SupportStatus.InProgress.ToString(), SupportStatus.Resolved.ToString());

            return ticket;
        }

        public async Task<UserSupportTicket> CloseTicketAsync(int ticketId)
        {
            var ticket = await _ticketRepo.GetByIdAsync(ticketId);
            if (ticket == null)
                throw new InvalidOperationException("Phiếu hỗ trợ không tồn tại");

            ticket.Status = SupportStatus.Closed;
            ticket.ResolvedAt = DateTime.UtcNow;

            await _ticketRepo.UpdateAsync(ticket);

            await _auditService.LogActionAsync("CLOSE", "UserSupportTicket", ticketId, ticket.UserId,
                ticket.Status.ToString(), SupportStatus.Closed.ToString());

            return ticket;
        }

        public async Task DeleteTicketAsync(int id)
        {
            var ticket = await _ticketRepo.GetByIdAsync(id);
            if (ticket == null)
                throw new InvalidOperationException("Phiếu hỗ trợ không tồn tại");

            await _ticketRepo.DeleteAsync(id);

            await _auditService.LogActionAsync("DELETE", "UserSupportTicket", id, "system",
                ticket.Status.ToString(), null);
        }
    }
}
