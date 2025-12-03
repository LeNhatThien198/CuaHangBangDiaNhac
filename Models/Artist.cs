using System.ComponentModel.DataAnnotations;

namespace CuaHangBangDiaNhac.Models
{
    public class Artist
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên nghệ sĩ")]
        [MaxLength(100)]
        [Display(Name = "Nghệ sĩ")]
        public string Name { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}