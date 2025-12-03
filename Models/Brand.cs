using System.ComponentModel.DataAnnotations;

namespace CuaHangBangDiaNhac.Models
{
    public class Brand
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên hãng")]
        [MaxLength(100)]
        [Display(Name = "Hãng đĩa/Nhà phát hành")]
        public string Name { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}