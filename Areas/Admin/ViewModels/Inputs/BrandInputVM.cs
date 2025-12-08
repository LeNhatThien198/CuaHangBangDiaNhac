using System.ComponentModel.DataAnnotations;

namespace CuaHangBangDiaNhac.Areas.Admin.ViewModels.Inputs
{
    public class BrandInputVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên hãng đĩa")]
        [Display(Name = "Hãng đĩa / Nhà phát hành")]
        public string Name { get; set; }
    }
}