using System.ComponentModel.DataAnnotations;

namespace CuaHangBangDiaNhac.Areas.Admin.ViewModels.Inputs
{
    public class CategoryInputVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên định dạng")]
        [Display(Name = "Định dạng (Vinyl/CD...)")]
        public string Name { get; set; }
    }
}