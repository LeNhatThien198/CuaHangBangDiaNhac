using CuaHangBangDiaNhac.Models;

namespace CuaHangBangDiaNhac.ViewModels
{
    public class HomeViewModel
    {
        public List<CarouselItem> CarouselItems { get; set; } = new List<CarouselItem>();
        public List<Product> NewArrivals { get; set; } = new List<Product>();
        public List<Product> FeaturedProducts { get; set; } = new List<Product>();
    }

    public class CarouselItem
    {
        public string ImageUrl { get; set; }
        public int? ProductId { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public bool IsPreOrder { get; set; }
    }
}
