using CuaHangBangDiaNhac.Areas.Admin.ViewModels;
using CuaHangBangDiaNhac.Data;
using CuaHangBangDiaNhac.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CuaHangBangDiaNhac.Areas.Admin.ViewModels.Inputs;

namespace CuaHangBangDiaNhac.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Staff")]
    public class MasterDataController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MasterDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string tab = "artist") 
        {
            ViewBag.ActiveTab = tab?.ToLower() ?? "artist";

            var model = new MasterDataVM
            {
                Artists = await _context.Artists.OrderByDescending(a => a.Id).ToListAsync(),
                Brands = await _context.Brands.OrderByDescending(b => b.Id).ToListAsync(),
                Categories = await _context.Categories.OrderByDescending(c => c.Id).ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> CreateArtist(MasterDataVM model)
        {
            if (!string.IsNullOrWhiteSpace(model.NewArtist.Name))
            {
                var artist = new Artist { Name = model.NewArtist.Name };
                _context.Artists.Add(artist);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã thêm nghệ sĩ thành công!";
            }
            return RedirectToAction("Index", new { tab = "artist" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteArtist(int id)
        {
            var item = await _context.Artists.FindAsync(id);
            if (item != null)
            {
                _context.Artists.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã xóa nghệ sĩ.";
            }
            return RedirectToAction("Index", new { tab = "artist" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBrand(MasterDataVM model)
        {
            if (!string.IsNullOrWhiteSpace(model.NewBrand.Name))
            {
                var brand = new Brand { Name = model.NewBrand.Name };
                _context.Brands.Add(brand);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã thêm hãng đĩa thành công!";
            }
            return RedirectToAction("Index", new { tab = "brand" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var item = await _context.Brands.FindAsync(id);
            if (item != null)
            {
                _context.Brands.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã xóa hãng đĩa.";
            }
            return RedirectToAction("Index", new { tab = "brand" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(MasterDataVM model)
        {
            if (!string.IsNullOrWhiteSpace(model.NewCategory.Name))
            {
                var category = new Category { Name = model.NewCategory.Name };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã thêm định dạng thành công!";
            }
            return RedirectToAction("Index", new { tab = "category" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var item = await _context.Categories.FindAsync(id);
            if (item != null)
            {
                _context.Categories.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã xóa định dạng.";
            }
            return RedirectToAction("Index", new { tab = "category" });
        }

        [HttpGet]
        public async Task<IActionResult> EditArtist(int id)
        {
            var artist = await _context.Artists.FindAsync(id);
            if (artist == null) return NotFound();

            var model = new ArtistInputVM { Id = artist.Id, Name = artist.Name };
            return PartialView("Partials/_EditArtistModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditArtist(ArtistInputVM model)
        {
            if (ModelState.IsValid)
            {
                var artist = await _context.Artists.FindAsync(model.Id);
                if (artist == null) return NotFound();

                artist.Name = model.Name;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật nghệ sĩ thành công.";
                return RedirectToAction("Index", new { tab = "artist" });
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditBrand(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null) return NotFound();

            var model = new BrandInputVM { Id = brand.Id, Name = brand.Name };
            return PartialView("Partials/_EditBrandModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBrand(BrandInputVM model)
        {
            if (ModelState.IsValid)
            {
                var brand = await _context.Brands.FindAsync(model.Id);
                if (brand == null) return NotFound();

                brand.Name = model.Name;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật hãng đĩa thành công.";
                return RedirectToAction("Index", new { tab = "brand" });
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            var cat = await _context.Categories.FindAsync(id);
            if (cat == null) return NotFound();

            var model = new CategoryInputVM { Id = cat.Id, Name = cat.Name };
            return PartialView("Partials/_EditCategoryModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(CategoryInputVM model)
        {
            if (ModelState.IsValid)
            {
                var cat = await _context.Categories.FindAsync(model.Id);
                if (cat == null) return NotFound();

                cat.Name = model.Name;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật định dạng thành công.";
                return RedirectToAction("Index", new { tab = "category" });
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetArtistsJson()
        {
            var data = await _context.Artists.Select(a => new { id = a.Id, name = a.Name }).ToListAsync();
            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult> QuickCreateArtist(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return Json(new { success = false, message = "Tên không được để trống" });

            var artist = new Artist { Name = name };
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();
            return Json(new { success = true, id = artist.Id, name = artist.Name });
        }

        [HttpGet]
        public async Task<IActionResult> GetBrandsJson()
        {
            var data = await _context.Brands.Select(b => new { id = b.Id, name = b.Name }).ToListAsync();
            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult> QuickCreateBrand(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return Json(new { success = false, message = "Tên không được để trống" });

            var brand = new Brand { Name = name };
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
            return Json(new { success = true, id = brand.Id, name = brand.Name });
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoriesJson()
        {
            var data = await _context.Categories.Select(c => new { id = c.Id, name = c.Name }).ToListAsync();
            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult> QuickCreateCategory(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return Json(new { success = false, message = "Tên không được để trống" });

            var cat = new Category { Name = name };
            _context.Categories.Add(cat);
            await _context.SaveChangesAsync();
            return Json(new { success = true, id = cat.Id, name = cat.Name });
        }
    }
}