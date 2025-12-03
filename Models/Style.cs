using System.ComponentModel.DataAnnotations;

namespace CuaHangBangDiaNhac.Models
{
    public class Style
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên phong cách/thể loại")]
        [MaxLength(100)]
        [Display(Name = "Phong cách")]
        public string Name { get; set; } 

        [Display(Name = "Thuộc thể loại")]
        public int GenreId { get; set; }
        public Genre? Genre { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}