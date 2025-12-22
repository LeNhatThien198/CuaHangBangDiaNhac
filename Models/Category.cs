using System.ComponentModel.DataAnnotations;

namespace CuaHangBangDiaNhac.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên định dạng")]
        [MaxLength(50, ErrorMessage = "{0} tối đa {1} ký tự")]
        [Display(Name = "Định dạng")] 
        public string Name { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}