using System;
using CuaHangBangDiaNhac.Models;

namespace CuaHangBangDiaNhac.Areas.Admin.ViewModels
{
    // ModeratorTicket Display VMs
    public class ModeratorTicketListVM
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ModeratorTicketDetailVM
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string ModeratorId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // UserSupportTicket Display VMs
    public class UserSupportTicketListVM
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserSupportTicketDetailVM
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string AssignedToId { get; set; }
        public string AssignedToName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }

    // AuditLog Display VM
    public class AuditLogListVM
    {
        public int Id { get; set; }
        public string Action { get; set; }
        public string EntityName { get; set; }
        public int? EntityId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
