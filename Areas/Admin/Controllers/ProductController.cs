using CuaHangBangDiaNhac.Areas.Admin.ViewModels;
using CuaHangBangDiaNhac.Areas.Admin.ViewModels.Inputs;
using CuaHangBangDiaNhac.Data;
using CuaHangBangDiaNhac.Models;
using CuaHangBangDiaNhac.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.RegularExpressions;

namespace CuaHangBangDiaNhac.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Staff")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // --- INDEX (GIỮ NGUYÊN) ---
        [HttpGet]
        public async Task<IActionResult> Index(string search, string sortOrder, int page = 1)
        {
            int pageSize = 10;
            var query = _context.Products
                .Include(p => p.Artist).Include(p => p.Category)
                .Include(p => p.Brand).Include(p => p.Genre).Include(p => p.Style)
                .Include(p => p.Images)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(search) || p.Artist.Name.ToLower().Contains(search));
            }

            switch (sortOrder)
            {
                case "name_asc": query = query.OrderBy(p => p.Name); break;
                case "name_desc": query = query.OrderByDescending(p => p.Name); break;
                case "price_asc": query = query.OrderBy(p => p.Price); break;
                case "price_desc": query = query.OrderByDescending(p => p.Price); break;
                case "id_asc": query = query.OrderBy(p => p.Id); break;
                case "status_asc": query = query.OrderBy(p => p.IsPublished); break;
                case "status_desc": query = query.OrderByDescending(p => p.IsPublished); break;
                case "condition_asc": query = query.OrderBy(p => p.IsUsed); break;
                case "condition_desc": query = query.OrderByDescending(p => p.IsUsed); break;
                case "preorder_desc": query = query.OrderByDescending(p => p.IsPreOrder); break;
                case "preorder_asc": query = query.OrderBy(p => p.IsPreOrder); break;
                default: query = query.OrderByDescending(p => p.Id); break;
            }

            int totalItems = await query.CountAsync();
            var products = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var vmList = products.Select(p => new ProductListVM
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                PromotionPrice = p.PromotionPrice,
                Cost = p.Cost,
                Quantity = p.Quantity,
                ArtistName = p.Artist?.Name ?? "---",
                BrandName = p.Brand?.Name ?? "---",
                CategoryName = p.Category?.Name ?? "---",
                GenreName = p.Genre?.Name ?? "---",
                StyleName = p.Style?.Name ?? "---",
                IsUsed = p.IsUsed,
                IsPreOrder = p.IsPreOrder,
                IsPublished = p.IsPublished,
                ImageUrl = p.Images.FirstOrDefault(img => img.IsPrimary)?.Url ?? p.Images.FirstOrDefault()?.Url
            }).ToList();

            ViewBag.Pagination = new Pagination { PageNumber = page, TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize) };
            ViewBag.Search = search;
            ViewBag.SortOrder = sortOrder;

            return View(vmList);
        }

        // --- CREATE ---
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            // Mặc định new product: Mới (IsUsed=false) -> Condition = Mint
            var vm = new ProductInputVM { IsUsed = false, Condition = "M (Mint)" };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductInputVM model)
        {
            if (ModelState.IsValid)
            {
                // Logic Backend bảo vệ: Nếu là Hàng Mới -> Bắt buộc Mint
                if (!model.IsUsed) model.Condition = "M (Mint)";

                var product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Tracklist = model.Tracklist,
                    Price = model.Price,
                    PromotionPrice = model.PromotionPrice,
                    Cost = model.Cost,
                    Quantity = model.Quantity,
                    IsPublished = model.IsPublished,
                    Condition = model.Condition,
                    Country = model.Country,
                    ReleaseYear = model.ReleaseYear,
                    IsUsed = model.IsUsed,
                    IsPreOrder = model.IsPreOrder,
                    ReleaseDate = model.ReleaseDate,
                    CreatedAt = DateTime.UtcNow,
                    ArtistId = model.ArtistId,
                    BrandId = model.BrandId,
                    CategoryId = model.CategoryId,
                    GenreId = model.GenreId,
                    StyleId = model.StyleId
                };

                var artistName = (await _context.Artists.FindAsync(model.ArtistId))?.Name ?? "Unknown";

                if (model.CoverImage != null)
                {
                    string coverPath = await SaveFile(model.CoverImage, artistName, model.Name, "cover");
                    product.Images.Add(new ProductImage { Url = coverPath, IsPrimary = true });
                }

                if (model.GalleryImages != null && model.GalleryImages.Any())
                {
                    foreach (var file in model.GalleryImages)
                    {
                        string path = await SaveFile(file, artistName, model.Name, "gallery");
                        product.Images.Add(new ProductImage { Url = path, IsPrimary = false });
                    }
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã thêm sản phẩm thành công!";
                return RedirectToAction("Index");
            }
            await LoadDropdowns();
            return View(model);
        }

        // --- EDIT ---
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            var model = new ProductInputVM
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Tracklist = product.Tracklist,
                Price = product.Price,
                PromotionPrice = product.PromotionPrice,
                Cost = product.Cost,
                Quantity = product.Quantity,
                IsPublished = product.IsPublished,
                Condition = product.Condition,
                Country = product.Country,
                ReleaseYear = product.ReleaseYear,
                IsUsed = product.IsUsed,
                IsPreOrder = product.IsPreOrder,
                ReleaseDate = product.ReleaseDate,
                ArtistId = product.ArtistId,
                BrandId = product.BrandId,
                CategoryId = product.CategoryId,
                GenreId = product.GenreId,
                StyleId = product.StyleId,
                CurrentCoverUrl = product.Images.FirstOrDefault(i => i.IsPrimary)?.Url,
                CurrentGallery = product.Images.Where(i => !i.IsPrimary).ToList()
            };

            await LoadDropdowns(product.GenreId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductInputVM model)
        {
            if (ModelState.IsValid)
            {
                var product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == model.Id);
                if (product == null) return NotFound();

                if (!model.IsUsed) model.Condition = "M (Mint)";

                product.Name = model.Name; product.Description = model.Description; product.Tracklist = model.Tracklist;
                product.Price = model.Price; product.PromotionPrice = model.PromotionPrice; product.Cost = model.Cost;
                product.Quantity = model.Quantity; product.IsPublished = model.IsPublished; product.Condition = model.Condition;
                product.Country = model.Country; product.ReleaseYear = model.ReleaseYear; product.IsUsed = model.IsUsed;
                product.IsPreOrder = model.IsPreOrder; product.ReleaseDate = model.ReleaseDate; product.UpdatedAt = DateTime.UtcNow;
                product.ArtistId = model.ArtistId; product.BrandId = model.BrandId; product.CategoryId = model.CategoryId;
                product.GenreId = model.GenreId; product.StyleId = model.StyleId;

                var artistName = (await _context.Artists.FindAsync(model.ArtistId))?.Name ?? "Unknown";

                // --- XỬ LÝ ẢNH BÌA (CÓ XÓA FILE CŨ) ---
                if (model.CoverImage != null)
                {
                    var oldCover = product.Images.FirstOrDefault(i => i.IsPrimary);
                    if (oldCover != null)
                    {
                        DeleteFile(oldCover.Url); // Xóa vật lý
                        _context.ProductImages.Remove(oldCover);
                    }
                    string coverPath = await SaveFile(model.CoverImage, artistName, model.Name, "cover");
                    product.Images.Add(new ProductImage { Url = coverPath, IsPrimary = true });
                }

                // --- XỬ LÝ GALLERY (THÊM MỚI) ---
                if (model.GalleryImages != null && model.GalleryImages.Any())
                {
                    foreach (var file in model.GalleryImages)
                    {
                        string path = await SaveFile(file, artistName, model.Name, "gallery");
                        product.Images.Add(new ProductImage { Url = path, IsPrimary = false });
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction("Index");
            }
            await LoadDropdowns(model.GenreId);
            return View(model);
        }

        // --- DELETE IMAGE (AJAX) - CÓ XÓA FILE VẬT LÝ ---
        [HttpPost]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var img = await _context.ProductImages.FindAsync(imageId);
            if (img != null)
            {
                DeleteFile(img.Url); // Xóa file trên ổ cứng
                _context.ProductImages.Remove(img); // Xóa trong DB
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        // --- DELETE PRODUCT ---
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
            if (product != null)
            {
                foreach (var img in product.Images) DeleteFile(img.Url); // Xóa sạch ảnh
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã xóa sản phẩm.";
            }
            return RedirectToAction("Index");
        }

        // --- HELPERS ---
        private async Task<string> SaveFile(IFormFile file, string artist, string product, string type)
        {
            string safeArtist = GetSafeFileName(artist);
            string safeProduct = GetSafeFileName(product);
            string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products", safeArtist, safeProduct, type);

            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            string fileName = "";
            if (type == "cover")
            {
                fileName = "c" + Path.GetExtension(file.FileName);
            }
            else
            {
                int maxIndex = 0;
                var existingFiles = Directory.GetFiles(folderPath, "g*.*");
                foreach (var f in existingFiles)
                {
                    var fname = Path.GetFileNameWithoutExtension(f);
                    if (fname.StartsWith("g") && int.TryParse(fname.Substring(1), out int idx))
                    {
                        if (idx > maxIndex) maxIndex = idx;
                    }
                }
                fileName = $"g{maxIndex + 1}{Path.GetExtension(file.FileName)}";
            }

            string filePath = Path.Combine(folderPath, fileName);
            // Nếu là cover, xóa file cũ trùng tên trước khi ghi mới để tránh lỗi lock hoặc cache
            if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/images/products/{safeArtist}/{safeProduct}/{type}/{fileName}";
        }

        private void DeleteFile(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return;
            string absolutePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath.TrimStart('/'));
            if (System.IO.File.Exists(absolutePath)) System.IO.File.Delete(absolutePath);
        }

        private string GetSafeFileName(string input)
        {
            if (string.IsNullOrEmpty(input)) return "unknown";
            string str = input.Normalize(NormalizationForm.FormD);
            var chars = str.Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark).ToArray();
            str = new string(chars).Normalize(NormalizationForm.FormC);
            str = Regex.Replace(str, @"[^a-zA-Z0-9\-]", "");
            return str;
        }

        private async Task LoadDropdowns(int? selectedGenreId = null)
        {
            ViewBag.Artists = new SelectList(await _context.Artists.OrderBy(a => a.Name).ToListAsync(), "Id", "Name");
            ViewBag.Brands = new SelectList(await _context.Brands.OrderBy(b => b.Name).ToListAsync(), "Id", "Name");
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            ViewBag.Genres = new SelectList(await _context.Genres.OrderBy(g => g.Name).ToListAsync(), "Id", "Name");

            // LIST CONDITION CHUẨN YÊU CẦU
            var conditions = new List<string> {
                "M (Mint)", "NM (Near Mint)", "EX (Excellent)", "VG+ (Very Good Plus)", "VG (Very Good)", "G (Good)", "F/P (Fair/Poor)"
            };
            ViewBag.Conditions = new SelectList(conditions);

            if (selectedGenreId.HasValue)
            {
                ViewBag.Styles = new SelectList(await _context.Styles.Where(s => s.GenreId == selectedGenreId).OrderBy(s => s.Name).ToListAsync(), "Id", "Name");
            }
            else
            {
                ViewBag.Styles = new SelectList(Enumerable.Empty<Style>(), "Id", "Name");
            }
        }
    }
}