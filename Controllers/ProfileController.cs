using CuaHangBangDiaNhac.Data;
using CuaHangBangDiaNhac.Models;
using CuaHangBangDiaNhac.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CuaHangBangDiaNhac.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProfileController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var isAdminOrStaff = User.IsInRole("Admin") || User.IsInRole("Staff");

            // Self-healing: invalid/missing CreatedAt for old users
            if (user.CreatedAt.Year < 2000)
            {
                user.CreatedAt = DateTime.Now;
                await _userManager.UpdateAsync(user);
            }

            var model = new ProfileViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                AvatarUrl = user.AvatarUrl,
                IsAdminOrStaff = isAdminOrStaff,
                CreatedAt = user.CreatedAt
            };

            if (isAdminOrStaff)
            {
                var address = await _context.Addresses.FirstOrDefaultAsync(a => a.UserId == user.Id && a.IsDefault);
                if (address != null)
                {
                    model.AddressLine = address.Line1;
                    model.Province = address.Province;
                    model.Ward = address.Line2;
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (ModelState.IsValid)
            {
                // Update Basic Info
                user.UserName = model.Username; 
                user.FullName = model.FullName;
                user.PhoneNumber = model.PhoneNumber;
                user.DateOfBirth = model.DateOfBirth;
                
                // Handle Email Update (Admin/Staff Only)
                if (User.IsInRole("Admin") || User.IsInRole("Staff"))
                {
                    user.Email = model.Email; 
                }

                // Handle Avatar Upload
                if (model.AvatarFile != null)
                {
                    var savedUrl = await SaveAvatarFile(user, model.AvatarFile);
                    user.AvatarUrl = savedUrl;
                }

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    // Handle Admin Address Update
                    if (User.IsInRole("Admin") || User.IsInRole("Staff"))
                    {
                        var address = await _context.Addresses.FirstOrDefaultAsync(a => a.UserId == user.Id && a.IsDefault);
                        if (address == null)
                        {
                            // Create new
                            address = new Address
                            {
                                UserId = user.Id,
                                ReceiverName = user.FullName ?? "Admin",
                                PhoneNumber = user.PhoneNumber ?? "",
                                Line1 = model.AddressLine ?? "",
                                Line2 = model.Ward ?? "",
                                Province = model.Province ?? "",
                                City = "Không có",
                                IsDefault = true
                            };
                            _context.Addresses.Add(address);
                        }
                        else
                        {
                            // Update existing
                            address.ReceiverName = user.FullName ?? "Admin"; // Sync name
                            address.PhoneNumber = user.PhoneNumber ?? "";   // Sync phone
                            address.Line1 = model.AddressLine ?? "";
                            address.Line2 = model.Ward ?? "";
                            address.Province = model.Province ?? "";
                            _context.Addresses.Update(address);
                        }
                        await _context.SaveChangesAsync();
                    }

                    TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
                    // Refresh model to show new avatar immediately
                    model.AvatarUrl = user.AvatarUrl;
                    model.IsAdminOrStaff = User.IsInRole("Admin") || User.IsInRole("Staff"); // Valid for return View
                    return View(model);
                }

                foreach (var error in result.Errors)
                {
                    if (error.Code == "DuplicateUserName")
                    {
                        ModelState.AddModelError("Username", $"Tên đăng nhập '{model.Username}' đã được sử dụng.");
                    }
                    else if (error.Code == "DuplicateEmail")
                    {
                        ModelState.AddModelError("Email", $"Email '{model.Email}' đã được sử dụng.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            model.Id = user.Id;
            model.Username = user.UserName;
            model.Email = user.Email;
            model.AvatarUrl = user.AvatarUrl; // Restore avatar if failed
            model.IsAdminOrStaff = User.IsInRole("Admin") || User.IsInRole("Staff");
            
            return View(model);
        }
        // --- Address Management ---
        [HttpGet]
        public async Task<IActionResult> Address(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var addresses = await _context.Addresses
                .Where(a => a.UserId == user.Id && !a.IsDeleted)
                .OrderByDescending(a => a.IsDefault)
                .ToListAsync();

            return View(addresses);
        }

        [HttpGet]
        public async Task<IActionResult> CreateAddress(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var count = await _context.Addresses.CountAsync(a => a.UserId == user.Id);
            if (count >= 2)
            {
                TempData["ErrorMessage"] = "Bạn chỉ có thể thêm tối đa 2 địa chỉ.";
                return RedirectToAction(nameof(Address));
            }

            return View(new Address { ReceiverName = user.FullName ?? "", PhoneNumber = user.PhoneNumber ?? "" });
        }

        [HttpGet]
        public async Task<IActionResult> EditAddress(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == user.Id);
            if (address == null) return NotFound();

            return View(address);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAddress(int id, Address model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (id != model.Id) return NotFound();

            var addressToUpdate = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == user.Id);
            if (addressToUpdate == null) return NotFound();

            // Clear validation for User/UserId/City
            ModelState.Remove("UserId");
            ModelState.Remove("User");
            ModelState.Remove("City");

            if (ModelState.IsValid)
            {
                addressToUpdate.ReceiverName = model.ReceiverName;
                addressToUpdate.PhoneNumber = model.PhoneNumber;
                addressToUpdate.Line1 = model.Line1;
                addressToUpdate.Line2 = model.Line2;
                addressToUpdate.Province = model.Province;
                // City is unused
                
                _context.Addresses.Update(addressToUpdate);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật địa chỉ thành công!";
                return RedirectToAction(nameof(Address));
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAddress(Address model, string? returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var count = await _context.Addresses.CountAsync(a => a.UserId == user.Id);
            if (count >= 2)
            {
                TempData["ErrorMessage"] = "Bạn chỉ có thể thêm tối đa 2 địa chỉ.";
                return RedirectToAction(nameof(Address));
            }

            // Manually re-validate model because UserId and User are required but not in form
            // We set them now
            model.UserId = user.Id;
            // Clear validation errors for User/UserId if any (though usually strictly checked if in ModelState)
            ModelState.Remove("UserId");
            ModelState.Remove("User");
            ModelState.Remove("User");
            ModelState.Remove("City"); // Handled as default
            model.City = ""; // Empty string for DB compatibility

            if (ModelState.IsValid)
            {
                // First address is always default
                if (count == 0) model.IsDefault = true;
                else model.IsDefault = false;

                _context.Addresses.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thêm địa chỉ thành công!";
                if (!string.IsNullOrEmpty(returnUrl)) return Redirect(returnUrl);
                return RedirectToAction(nameof(Address));
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDefaultAddress(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == user.Id);
            if (address == null) return NotFound();

            // Unset current default
            var currentDefault = await _context.Addresses.FirstOrDefaultAsync(a => a.UserId == user.Id && a.IsDefault);
            if (currentDefault != null)
            {
                currentDefault.IsDefault = false;
                _context.Addresses.Update(currentDefault);
            }

            // Set new default
            address.IsDefault = true;
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã thay đổi địa chỉ mặc định.";
            return RedirectToAction(nameof(Address));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == user.Id);
            if (address == null) return NotFound();

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            // If deleted address was default, and another exists, make it default
            var remaining = await _context.Addresses.Where(a => a.UserId == user.Id).ToListAsync();
            if (remaining.Count == 1 && !remaining[0].IsDefault)
            {
                remaining[0].IsDefault = true;
                _context.Addresses.Update(remaining[0]);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Xóa địa chỉ thành công!";
            return RedirectToAction(nameof(Address));
        }
        // --- Change Password ---
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var hasPassword = await _userManager.HasPasswordAsync(user);
            ViewBag.HasPassword = hasPassword;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
                return RedirectToAction(nameof(ChangePassword));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UploadAvatar(IFormFile avatarFile)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            if (avatarFile == null || avatarFile.Length == 0)
                return BadRequest(new { success = false, message = "Chưa chọn file ảnh." });

            var savedUrl = await SaveAvatarFile(user, avatarFile);
            user.AvatarUrl = savedUrl;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                return Json(new { success = true, url = savedUrl });
            }

            var error = result.Errors.FirstOrDefault()?.Description ?? "Cập nhật thất bại.";
            return BadRequest(new { success = false, message = error });
        }

        private async Task<string> SaveAvatarFile(User user, IFormFile avatarFile)
        {
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "users");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            string extension = Path.GetExtension(avatarFile.FileName);
            string username = user.UserName ?? "user";

            int nextIndex = 1;
            var existingFiles = Directory.GetFiles(uploadsFolder, $"{username}_*{extension}");
            foreach (var file in existingFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var parts = fileName.Split('_');
                var lastPart = parts.LastOrDefault();
                if (int.TryParse(lastPart, out int idx) && idx >= nextIndex) nextIndex = idx + 1;
            }

            string newFileName = $"{username}_{nextIndex}{extension}";
            string filePath = Path.Combine(uploadsFolder, newFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await avatarFile.CopyToAsync(fileStream);
            }

            return "/images/users/" + newFileName;
        }
    }
}
