using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CuaHangBangDiaNhac.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập Email hoặc tên đăng nhập")]
    [Display(Name = "Email hoặc Tên đăng nhập")]
    public string EmailOrUsername { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; }

    [Display(Name = "Ghi nhớ tôi")]
    public bool RememberMe { get; set; }
    public string? ReturnUrl { get; set; }
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
    [Display(Name = "Tên đăng nhập")]
    [Remote("VerifyUsername", "Account")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ Email")]
    [EmailAddress]
    [Display(Name = "Email")]
    [Remote("VerifyEmail", "Account")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; }

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
    [Display(Name = "Số điện thoại liên hệ")]
    public string PhoneNumber { get; set; }
}

public class ExternalLoginConfirmViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email liên kết")]
    public string Email { get; set; } 

    [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } 

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [Display(Name = "Số điện thoại liên hệ")]
    public string PhoneNumber { get; set; }
}

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập Email")]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }
}

public class ResetPasswordViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email xác nhận")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
    [StringLength(100,
    MinimumLength = 6,
    ErrorMessage = "{0} phải có ít nhất {2} ký tự, gồm chữ hoa, chữ thường, chữ số và ký tự đặc biệt.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu mới")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Nhập lại mật khẩu")]
    [Compare("Password", ErrorMessage = "Mật khẩu nhập lại không khớp")]
    public string ConfirmPassword { get; set; }

    public string Code { get; set; }
}



