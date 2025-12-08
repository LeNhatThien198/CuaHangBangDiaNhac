using CuaHangBangDiaNhac.Areas.Admin.ViewModels;
using CuaHangBangDiaNhac.Areas.Admin.ViewModels.Inputs;
using CuaHangBangDiaNhac.Data;
using CuaHangBangDiaNhac.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CuaHangBangDiaNhac.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Staff")]
    public class GenreController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GenreController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new GenreManagerVM
            {
                Genres = await _context.Genres
                                .Include(g => g.Styles)
                                .OrderBy(g => g.Name)
                                .ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGenre(GenreManagerVM model)
        {
            if (!string.IsNullOrWhiteSpace(model.NewGenre.Name))
            {
                var genre = new Genre { Name = model.NewGenre.Name };
                _context.Genres.Add(genre);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã thêm thể loại mới.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre != null)
            {
                _context.Genres.Remove(genre);
                await _context.SaveChangesAsync(); 
                TempData["SuccessMessage"] = "Đã xóa Thể loại.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStyle(GenreManagerVM model)
        {
            if (!string.IsNullOrWhiteSpace(model.NewStyle.Name) && model.NewStyle.GenreId > 0)
            {
                var style = new Style
                {
                    Name = model.NewStyle.Name,
                    GenreId = model.NewStyle.GenreId
                };
                _context.Styles.Add(style);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã thêm phong cách mới.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStyle(int id)
        {
            var style = await _context.Styles.FindAsync(id);
            if (style != null)
            {
                _context.Styles.Remove(style);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã xóa Phong cách.";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditGenre(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null) return NotFound();

            var model = new GenreInputVM { Id = genre.Id, Name = genre.Name };
            return PartialView("Partials/_EditGenreModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGenre(GenreInputVM model)
        {
            if (ModelState.IsValid)
            {
                var genre = await _context.Genres.FindAsync(model.Id);
                if (genre == null) return NotFound();

                genre.Name = model.Name;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật Thể loại thành công.";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index"); 
        }

        [HttpGet]
        public async Task<IActionResult> EditStyle(int id)
        {
            var style = await _context.Styles.FindAsync(id);
            if (style == null) return NotFound();

            var model = new StyleInputVM { Id = style.Id, Name = style.Name, GenreId = style.GenreId };
            return PartialView("Partials/_EditStyleModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStyle(StyleInputVM model)
        {
            if (ModelState.IsValid)
            {
                var style = await _context.Styles.FindAsync(model.Id);
                if (style == null) return NotFound();

                style.Name = model.Name;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật Phong cách thành công.";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetGenresJson()
        {
            var data = await _context.Genres.Select(g => new { id = g.Id, name = g.Name }).ToListAsync();
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetStylesByGenreJson(int genreId)
        {
            var data = await _context.Styles
                .Where(s => s.GenreId == genreId)
                .Select(s => new { id = s.Id, name = s.Name })
                .ToListAsync();
            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult> QuickCreateGenre(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return Json(new { success = false, message = "Tên không được để trống" });

            var genre = new Genre { Name = name };
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();
            return Json(new { success = true, id = genre.Id, name = genre.Name });
        }

        [HttpPost]
        public async Task<IActionResult> QuickCreateStyle(string name, int genreId)
        {
            if (string.IsNullOrWhiteSpace(name) || genreId <= 0) return Json(new { success = false, message = "Dữ liệu không hợp lệ" });

            var style = new Style { Name = name, GenreId = genreId };
            _context.Styles.Add(style);
            await _context.SaveChangesAsync();
            return Json(new { success = true, id = style.Id, name = style.Name });
        }
    }
}