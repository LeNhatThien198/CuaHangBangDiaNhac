using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CuaHangBangDiaNhac.Models
{
    public class User : IdentityUser
    {
        [MaxLength(100)]
        [Display(Name = "Họ và tên")]
        public string? FullName { get; set; }

        [Display(Name = "Ngày sinh")]
        public DateTime? DateOfBirth { get; set; }

        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}