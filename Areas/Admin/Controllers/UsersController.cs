using CuaHangBangDiaNhac.Areas.Admin.ViewModels;
using CuaHangBangDiaNhac.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CuaHangBangDiaNhac.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] 
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager; 

        public UsersController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userVMs = new List<UserListVM>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userVMs.Add(new UserListVM
                {
                    Id = user.Id,
                    FullName = user.FullName ?? "Chưa cập nhật",
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber ?? "---",
                    Role = roles.FirstOrDefault() ?? "Customer", 
                    IsLocked = await _userManager.IsLockedOutAsync(user) 
                });
            }

            return View(userVMs);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Roles = new SelectList(new[] { "Admin", "Staff", "Customer" });
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserVM model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                    TempData["SuccessMessage"] = $"Đã tạo tài khoản {user.UserName} thành công!";
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            ViewBag.Roles = new SelectList(new[] { "Admin", "Staff", "Customer" }, model.Role);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var model = new EditUserVM
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = roles.FirstOrDefault() ?? "Customer", 
                IsLocked = await _userManager.IsLockedOutAsync(user)
            };

            ViewBag.Roles = new SelectList(new[] { "Admin", "Staff", "Customer" }, model.Role);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null) return NotFound();

                user.FullName = model.FullName;
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);

                    if (!currentRoles.Contains(model.Role))
                    {
                        await _userManager.RemoveFromRolesAsync(user, currentRoles); 
                        await _userManager.AddToRoleAsync(user, model.Role); 
                    }

                    if (model.IsLocked)
                    {
                        await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                    }
                    else
                    {
                        await _userManager.SetLockoutEndDateAsync(user, null);
                    }

                    var currentUserId = _userManager.GetUserId(User);

                    if (user.Id == currentUserId && (model.IsLocked || model.Role != "Admin"))
                    {
                        await _signInManager.SignOutAsync(); 
                        return RedirectToAction("Login", "Account", new { area = "" });
                    }

                    TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            ViewBag.Roles = new SelectList(new[] { "Admin", "Staff", "Customer" }, model.Role);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var currentUserId = _userManager.GetUserId(User);
                bool isSelfDelete = user.Id == currentUserId;

                await _userManager.DeleteAsync(user);

                if (isSelfDelete)
                {
                    await _signInManager.SignOutAsync();
                    return RedirectToAction("Login", "Account", new { area = "" });
                }

                TempData["SuccessMessage"] = "Đã xóa tài khoản vĩnh viễn.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
            }
            return RedirectToAction("Index");
        }
    }
}