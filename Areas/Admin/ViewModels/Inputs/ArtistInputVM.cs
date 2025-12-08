using System.ComponentModel.DataAnnotations;

namespace CuaHangBangDiaNhac.Areas.Admin.ViewModels.Inputs
{
    public class ArtistInputVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên nghệ sĩ")]
        [Display(Name = "Tên nghệ sĩ")]
        public string Name { get; set; }
    }
}