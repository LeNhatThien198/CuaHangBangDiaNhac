using CuaHangBangDiaNhac.Areas.Admin.ViewModels.Inputs;
using CuaHangBangDiaNhac.Models;

namespace CuaHangBangDiaNhac.Areas.Admin.ViewModels
{
    public class MasterDataVM
    {
        public List<Artist> Artists { get; set; } = new();
        public List<Brand> Brands { get; set; } = new();
        public List<Category> Categories { get; set; } = new();

        public ArtistInputVM NewArtist { get; set; } = new();
        public BrandInputVM NewBrand { get; set; } = new();
        public CategoryInputVM NewCategory { get; set; } = new();
    }
}
