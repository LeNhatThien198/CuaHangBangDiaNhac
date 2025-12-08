using System.ComponentModel.DataAnnotations;

namespace CuaHangBangDiaNhac.Areas.Admin.ViewModels.Inputs
{
    public class GenreInputVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên thể loại")]
        [Display(Name = "Tên thể loại")]
        public string Name { get; set; }
    }
}