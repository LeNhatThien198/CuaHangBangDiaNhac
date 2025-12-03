using System.ComponentModel.DataAnnotations;

namespace CuaHangBangDiaNhac.Models 
{
    public class Address
    {
        public int Id { get; set; }

        public string? UserId { get; set; }
        public User? User { get; set; }

        [Required, MaxLength(200)]
        public required string Line1 { get; set; } 

        [MaxLength(200)]
        public string? Line2 { get; set; }

        [Required, MaxLength(100)]
        public string? City { get; set; } 
        [MaxLength(100)]
        public string? Province { get; set; } 

        public bool IsDefault { get; set; } = false;
    }
}