using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CuaHangBangDiaNhac.Models
{
    public enum SupportStatus
    {
        [Display(Name = "Mở")]
        Open,
        [Display(Name = "Đang xử lý")]
        InProgress,
        [Display(Name = "Đã giải quyết")]
        Resolved,
        [Display(Name = "Đã đóng")]
        Closed
    }

    public enum SupportPriority
    {
        [Display(Name = "Thấp")]
        Low,
        [Display(Name = "Trung bình")]
        Medium,
        [Display(Name = "Cao")]
        High
    }

    public class UserSupportTicket
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [MaxLength(200, ErrorMessage = "Tiêu đề tối đa 200 ký tự")]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Mô tả không được để trống")]
        [MaxLength(2000, ErrorMessage = "Mô tả tối đa 2000 ký tự")]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public SupportStatus Status { get; set; } = SupportStatus.Open;

        [Required]
        [Display(Name = "Mức độ ưu tiên")]
        public SupportPriority Priority { get; set; } = SupportPriority.Medium;

        [Required]
        [Display(Name = "Khách hàng")]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Display(Name = "Được giao cho")]
        public string? AssignedToId { get; set; }

        [ForeignKey("AssignedToId")]
        public User? AssignedTo { get; set; }

        [Required]
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Ngày giải quyết")]
        public DateTime? ResolvedAt { get; set; }
    }
}
