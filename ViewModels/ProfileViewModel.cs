using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace CuaHangBangDiaNhac.ViewModels
{
    public class ProfileViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [Display(Name = "Tên đăng nhập")]
        [Remote(action: "VerifyUsername", controller: "Account", AdditionalFields = "Id")]
        public string? Username { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        [Remote(action: "VerifyEmail", controller: "Account", AdditionalFields = "Id")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [Display(Name = "Họ và tên")]
        [StringLength(100, ErrorMessage = "{0} không được vượt quá {1} ký tự.")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public string? AvatarUrl { get; set; }

        [Display(Name = "Tải ảnh mới")]
        public IFormFile? AvatarFile { get; set; }

        // --- Admin/Staff Single Address ---
        [MaxLength(100, ErrorMessage = "{0} tối đa {1} ký tự")]
        [Display(Name = "Phường / Xã")]
        public string? Ward { get; set; }

        [MaxLength(200, ErrorMessage = "{0} tối đa {1} ký tự")]
        [Display(Name = "Địa chỉ")]
        public string? AddressLine { get; set; }

        [MaxLength(100, ErrorMessage = "{0} tối đa {1} ký tự")]
        [Display(Name = "Tỉnh / Thành phố")]
        public string? Province { get; set; }
        
        // Helper to check if user is admin/staff in view
        public bool IsAdminOrStaff { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
