using CuaHangBangDiaNhac.Models;
using CuaHangBangDiaNhac.ViewModels.Shared; // Added Namespace

namespace CuaHangBangDiaNhac.ViewModels
{
    // Renamed to avoid conflict with Admin.ViewModels.ProductListVM
    public class StoreProductListVM
    {
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
        
        // Added properties to match the view's needs if they weren't matched before
        public Pagination? Pagination { get; set; } 
        public int TotalItems { get; set; } // Added for display count

        public List<Genre> Genres { get; set; } = new List<Genre>();
        public List<Artist> Artists { get; set; } = new List<Artist>();
        public List<Brand> Brands { get; set; } = new List<Brand>();
        public List<Category> Categories { get; set; } = new List<Category>();

        public string? Search { get; set; }
        public string? SortOrder { get; set; }
        public int? GenreId { get; set; }
        public int? StyleId { get; set; }
        public int? ArtistId { get; set; }
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }

        public bool? IsUsed { get; set; }
        public string? Condition { get; set; }
        public List<string> AvailableConditions { get; set; } = new List<string>();
    }

    public class ProductDetailVM
    {
        public Product Product { get; set; }
        public List<Product> RelatedProducts { get; set; } = new List<Product>();
    }
}
