using System.ComponentModel.DataAnnotations;

namespace CuaHangBangDiaNhac.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên thể loại")]
        [MaxLength(100, ErrorMessage = "{0} tối đa {1} ký tự")]
        [Display(Name = "Thể loại chính")]
        public string Name { get; set; } 

        public ICollection<Style> Styles { get; set; } = new List<Style>();

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}