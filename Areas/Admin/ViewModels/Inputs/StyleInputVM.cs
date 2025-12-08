using System.ComponentModel.DataAnnotations;

namespace CuaHangBangDiaNhac.Areas.Admin.ViewModels.Inputs
{
    public class StyleInputVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên phong cách/thể loại")]
        [Display(Name = "Tên phong cách")]
        public string Name { get; set; }

        [Required]
        public int GenreId { get; set; }
    }
}