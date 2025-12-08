using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CuaHangBangDiaNhac.Areas.Admin.ViewModels;

    public class UserListVM
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Role { get; set; } 
        public bool IsLocked { get; set; } 
    }

    public class CreateUserVM
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [Display(Name = "Tên đăng nhập")]
        [Remote(action: "VerifyUsername", controller: "Account", areaName: "")]

        public string UserName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ Email")]
        [EmailAddress]
        [Display(Name = "Email")]
        [Remote(action: "VerifyEmail", controller: "Account", areaName: "")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(100,
        MinimumLength = 6,
        ErrorMessage = "{0} phải có ít nhất {2} ký tự, gồm chữ hoa, chữ thường, chữ số và ký tự đặc biệt.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu nhập lại không khớp")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn vai trò")]
        [Display(Name = "Vai trò")]
        public string Role { get; set; } 
    }
 
    public class EditUserVM
    {
        public string Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
    [Display(Name = "Tên đăng nhập")]
    [Remote(action: "VerifyUsername", controller: "Account", areaName: "", AdditionalFields = nameof(Id))]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ Email")]
    [EmailAddress]
    [Display(Name = "Email")]
    [Remote(action: "VerifyEmail", controller: "Account", areaName: "", AdditionalFields = nameof(Id))]
    public string Email { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [Display(Name = "Số điện thoại")]
    public string PhoneNumber { get; set; }

    [Required]
    [Display(Name = "Vai trò")]
    public string Role { get; set; }

    [Display(Name = "Khóa tài khoản (Ban)")]
    public bool IsLocked { get; set; }
}