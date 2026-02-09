using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CuaHangBangDiaNhac.Models
{
    public enum TicketStatus
    {
        [Display(Name = "Chờ xử lý")]
        Pending,
        [Display(Name = "Phê duyệt")]
        Approved,
        [Display(Name = "Từ chối")]
        Rejected
    }

    public class ModeratorTicket
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public TicketStatus Status { get; set; } = TicketStatus.Pending;

        [MaxLength(500, ErrorMessage = "Lý do tối đa 500 ký tự")]
        [Display(Name = "Lý do")]
        public string? Reason { get; set; }

        [Required]
        [Display(Name = "Sản phẩm")]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        [Display(Name = "Người kiểm duyệt")]
        public string? ModeratorId { get; set; }

        [ForeignKey("ModeratorId")]
        public User? Moderator { get; set; }

        [Required]
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Ngày giải quyết")]
        public DateTime? ResolvedAt { get; set; }
    }
}
