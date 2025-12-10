using System.Diagnostics;
using CuaHangBangDiaNhac.Models;
using CuaHangBangDiaNhac.ViewModels;
using CuaHangBangDiaNhac.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CuaHangBangDiaNhac.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new HomeViewModel();

            // 1. Setup Carousel Data
            // Logic: Try to find product by specific keywords to link, otherwise null link
            var bannerProduct1 = await _context.Products.FirstOrDefaultAsync(p => p.Name.Contains("KIM") && p.Artist.Name.Contains("Chillies"));
            var bannerProduct2 = await _context.Products.FirstOrDefaultAsync(p => p.Name.Contains("Nevermind"));
            var bannerProduct3 = await _context.Products.FirstOrDefaultAsync(p => p.Name.Contains("Hybrid Theory"));

            vm.CarouselItems = new List<CarouselItem>
            {
                new CarouselItem {
                    ImageUrl = "/images/products/Chillies/KIM/banner/b.jpg",
                    ProductId = bannerProduct1?.Id,
                    Title = "KIM - Chillies",
                    Subtitle = "Album mới nhất với những giai điệu đầy cảm xúc.",
                    IsPreOrder = bannerProduct1?.IsPreOrder ?? false
                },
                new CarouselItem {
                    ImageUrl = "/images/products/Nirvana/Nevermind(Remastered)/banner/b.jpg",
                    ProductId = bannerProduct2?.Id,
                    Title = "Nevermind - Nirvana",
                    Subtitle = "Tác phẩm kinh điển định hình dòng nhạc Grunge.",
                    IsPreOrder = bannerProduct2?.IsPreOrder ?? false
                },
                new CarouselItem {
                    ImageUrl = "/images/products/LinkinPark/HybridTheory(20thAnniversaryEdition)/banner/b.jpg",
                    ProductId = bannerProduct3?.Id,
                    Title = "Hybrid Theory - Linkin Park",
                    Subtitle = "Kỷ niệm 20 năm album huyền thoại Nu-metal.",
                    IsPreOrder = bannerProduct3?.IsPreOrder ?? false
                }
            };

            // 2. New Arrivals (Latest 4 items - Reduced count)
            vm.NewArrivals = await _context.Products
                .Include(p => p.Artist)
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Where(p => p.IsPublished)
                .OrderByDescending(p => p.CreatedAt)
                .Take(4)
                .ToListAsync();

            // 3. Featured -> Best Selling Products
            // Strategy: Get top 4 products by Sold Quantity. if not enough, fill with Premium products.
            
            // A. Get Best Seller Ids
            var bestSellerIds = await _context.OrderItems
                .GroupBy(oi => oi.ProductId)
                .Select(g => new { ProductId = g.Key, Sold = g.Sum(oi => oi.Quantity) }) // Sum Quantity
                .OrderByDescending(x => x.Sold)
                .Take(4)
                .Select(x => x.ProductId)
                .ToListAsync();

            var featuredProducts = new List<Product>();

            // B. Fetch actual Product objects for Best Sellers
            if (bestSellerIds.Any())
            {
                var bestSellers = await _context.Products
                    .Include(p => p.Artist)
                    .Include(p => p.Images)
                    .Include(p => p.Category)
                    .Where(p => bestSellerIds.Contains(p.Id))
                    .ToListAsync();

                // Sort them to match the sales order (because database IN query doesn't preserve order)
                featuredProducts.AddRange(
                    bestSellers.OrderBy(p => bestSellerIds.IndexOf(p.Id))
                );
            }

            // C. Fallback: If < 4 items, fill with "Premium" (High Price) items to keep UI full
            if (featuredProducts.Count < 4)
            {
                var existingIds = featuredProducts.Select(p => p.Id).ToList();
                var fallbackCount = 4 - featuredProducts.Count;

                var fallbackProducts = await _context.Products
                    .Include(p => p.Artist)
                    .Include(p => p.Images)
                    .Include(p => p.Category)
                    .Where(p => p.IsPublished && !existingIds.Contains(p.Id)) // Exclude already added
                    .OrderByDescending(p => p.Price) // Show expensive items as 'Featured' fallback
                    .Take(fallbackCount)
                    .ToListAsync();
                
                featuredProducts.AddRange(fallbackProducts);
            }

            vm.FeaturedProducts = featuredProducts;

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
