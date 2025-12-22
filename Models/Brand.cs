using System.ComponentModel.DataAnnotations;

namespace CuaHangBangDiaNhac.Models
{
    public class Brand
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên hãng")]
        [MaxLength(100, ErrorMessage = "{0} tối đa {1} ký tự")]
        [Display(Name = "Hãng đĩa/Nhà phát hành")]
        public string Name { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}