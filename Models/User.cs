using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CuaHangBangDiaNhac.Models
{
    public class User : IdentityUser
    {
        [MaxLength(100, ErrorMessage = "{0} tối đa {1} ký tự")]
        [Display(Name = "Họ và tên")]
        public string? FullName { get; set; }

        [Display(Name = "Ngày sinh")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public string? AvatarUrl { get; set; } = "/images/default-user.png";

        [Display(Name = "Ngày tham gia")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}