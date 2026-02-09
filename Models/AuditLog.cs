using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CuaHangBangDiaNhac.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Hành động không được để trống")]
        [MaxLength(100, ErrorMessage = "Hành động tối đa 100 ký tự")]
        [Display(Name = "Hành động")]
        public string Action { get; set; }

        [Required(ErrorMessage = "Tên thực thể không được để trống")]
        [MaxLength(100, ErrorMessage = "Tên thực thể tối đa 100 ký tự")]
        [Display(Name = "Tên thực thể")]
        public string EntityName { get; set; }

        [Display(Name = "ID thực thể")]
        public int? EntityId { get; set; }

        [MaxLength(500, ErrorMessage = "Giá trị cũ tối đa 500 ký tự")]
        [Display(Name = "Giá trị cũ")]
        public string? OldValue { get; set; }

        [MaxLength(500, ErrorMessage = "Giá trị mới tối đa 500 ký tự")]
        [Display(Name = "Giá trị mới")]
        public string? NewValue { get; set; }

        [Display(Name = "Người dùng")]
        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        [Display(Name = "Thời gian")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [MaxLength(45, ErrorMessage = "Địa chỉ IP tối đa 45 ký tự")]
        [Display(Name = "Địa chỉ IP")]
        public string? IPAddress { get; set; }
    }
}
