using System.ComponentModel.DataAnnotations;

namespace CuaHangBangDiaNhac.Models 
{
    public class Address
    {
        public int Id { get; set; }

        public string? UserId { get; set; }
        public User? User { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập {0}")]
        [MaxLength(100, ErrorMessage = "{0} tối đa {1} ký tự")]
        [Display(Name = "Tên người nhận")]
        public string ReceiverName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập {0}")]
        [MaxLength(20, ErrorMessage = "{0} tối đa {1} ký tự")]
        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập {0}")]
        [MaxLength(200, ErrorMessage = "{0} tối đa {1} ký tự")]
        [Display(Name = "Địa chỉ chi tiết")]
        public string Line1 { get; set; } = string.Empty; 

        [MaxLength(200, ErrorMessage = "{0} tối đa {1} ký tự")]
        [Display(Name = "Phường/Xã (Tùy chọn)")]
        public string? Line2 { get; set; }

        [MaxLength(100, ErrorMessage = "{0} tối đa {1} ký tự")]
        [Display(Name = "Tỉnh/Thành phố")]
        public string? City { get; set; } 
        [MaxLength(100, ErrorMessage = "{0} tối đa {1} ký tự")]
        [Display(Name = "Quận/Huyện")]
        public string? Province { get; set; } 

        public bool IsDefault { get; set; } = false;
        
        public bool IsDeleted { get; set; } = false;
    }
}