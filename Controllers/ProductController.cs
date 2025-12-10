using CuaHangBangDiaNhac.Data;
using CuaHangBangDiaNhac.Models;
using CuaHangBangDiaNhac.ViewModels;
using CuaHangBangDiaNhac.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CuaHangBangDiaNhac.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Product
        public async Task<IActionResult> Index(string? search, string? sortOrder, int? categoryId, string? category, int? genreId, int? styleId, int? artistId, int? brandId, bool? isUsed, string? condition, int page = 1)
        {
            int pageSize = 12; 

            var query = _context.Products
                .Include(p => p.Artist)
                .Include(p => p.Brand)
                .Include(p => p.Images)
                .Include(p => p.Genre)
                .Include(p => p.Style)
                .Include(p => p.Category)
                .Where(p => p.IsPublished);

            // 1. Filtering
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.Artist.Name.Contains(search));
            }

            if (category != null && !categoryId.HasValue)
            {
                var catObj = await _context.Categories.FirstOrDefaultAsync(c => c.Name.Contains(category));
                if (catObj != null) categoryId = catObj.Id;
            }

            if (categoryId.HasValue) query = query.Where(p => p.CategoryId == categoryId.Value);
            if (genreId.HasValue) query = query.Where(p => p.GenreId == genreId.Value);
            if (styleId.HasValue) query = query.Where(p => p.StyleId == styleId.Value);
            if (artistId.HasValue) query = query.Where(p => p.ArtistId == artistId.Value);
            if (brandId.HasValue) query = query.Where(p => p.BrandId == brandId.Value);

            // New Filters
            if (isUsed.HasValue) 
            {
                query = query.Where(p => p.IsUsed == isUsed.Value);
            }

            if (!string.IsNullOrEmpty(condition))
            {
                query = query.Where(p => p.Condition == condition);
            }

            // 2. Sorting
            ViewData["CurrentSort"] = sortOrder;
            query = sortOrder switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "name_asc" => query.OrderBy(p => p.Name),
                "name_desc" => query.OrderByDescending(p => p.Name),
                "date_asc" => query.OrderBy(p => p.CreatedAt),
                "condition_new" => query.OrderBy(p => p.IsUsed).ThenByDescending(p => p.CreatedAt),
                "condition_used" => query.OrderByDescending(p => p.IsUsed).ThenByDescending(p => p.CreatedAt),
                "preorder" => query.OrderByDescending(p => p.IsPreOrder).ThenByDescending(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt) // Default: Newest
            };

            // 3. Pagination
            var totalItems = await query.CountAsync();
            var products = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // 4. Sidebar Data
            var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            var genres = await _context.Genres.Include(g => g.Styles).ToListAsync();
            var artists = await _context.Artists.OrderBy(a => a.Name).ToListAsync();
            var brands = await _context.Brands.OrderBy(b => b.Name).ToListAsync();

            // Get distinct conditions for Used items only
            var availableConditions = await _context.Products
                .Where(p => p.IsPublished && p.IsUsed && !string.IsNullOrEmpty(p.Condition))
                .Select(p => p.Condition)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            var vm = new StoreProductListVM
            {
                Products = products,
                Categories = categories,
                Genres = genres,
                Artists = artists,
                Brands = brands,
                AvailableConditions = availableConditions!, // Can use ! here as we filtered null/empty
                TotalItems = totalItems,
                Pagination = new Pagination
                {
                    TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                    PageNumber = page
                },
                Search = search,
                SortOrder = sortOrder,
                CategoryId = categoryId,
                GenreId = genreId,
                StyleId = styleId,
                ArtistId = artistId,
                BrandId = brandId,
                IsUsed = isUsed,
                Condition = condition
            };

            return View(vm);
        }

        // GET: /Product/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.Artist)
                .Include(p => p.Images)
                .Include(p => p.Genre)
                .Include(p => p.Style)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null || !product.IsPublished)
            {
                return NotFound();
            }

            // Related Products (Same Genre, exclude current)
            var related = await _context.Products
                .Include(p => p.Artist)
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Where(p => p.IsPublished && p.GenreId == product.GenreId && p.Id != id)
                .OrderBy(r => Guid.NewGuid()) // Randomize
                .Take(4)
                .ToListAsync();

            var vm = new ProductDetailVM
            {
                Product = product,
                RelatedProducts = related
            };

            return View(vm);
        }
    }
}
