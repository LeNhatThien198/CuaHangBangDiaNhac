using CuaHangBangDiaNhac.Models;
using CuaHangBangDiaNhac.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CuaHangBangDiaNhac.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.EmailOrUsername)
                           ?? await _userManager.FindByNameAsync(model.EmailOrUsername);

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(
                        user.UserName,
                        model.Password,
                        model.RememberMe,
                        lockoutOnFailure: true);

                    if (result.Succeeded)
                    {
                        return LocalRedirect(returnUrl);
                    }

                    if (result.IsLockedOut)
                    {
                        ViewData["BannedAccount"] = true;
                        return View(model);
                    }

                }
                ModelState.AddModelError(string.Empty, "Tài khoản hoặc mật khẩu không chính xác.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new User
                { UserName = model.Username, Email = model.Email, FullName = model.FullName, PhoneNumber = model.PhoneNumber };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Customer"); 
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
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
            return View(model);
        }


        [HttpPost]
        public IActionResult GoogleLogin(string? returnUrl = null)
        {
            var redirectUrl = Url.Action("GoogleResponse", "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return new ChallengeResult("Google", properties);
        }

        public async Task<IActionResult> GoogleResponse(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) return RedirectToAction(nameof(Login));

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded) return LocalRedirect(returnUrl);

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);

            var model = new ExternalLoginConfirmViewModel
            {
                Email = email,
                FullName = name
            };

            ViewData["ReturnUrl"] = returnUrl;
            return View("ExternalLoginConfirmation", model);
        }

        [HttpPost]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmViewModel model, string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null) return RedirectToAction(nameof(Login));

                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Customer");
                    result = await _userManager.AddLoginAsync(user, info);

                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [AcceptVerbs("GET", "POST")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyUsername(string username, string? id)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user != null)
            {
                if (!string.IsNullOrEmpty(id) && user.Id == id)
                {
                    return Json(true);
                }

                return Json($"Tên đăng nhập '{username}' đã được sử dụng.");
            }
            return Json(true);
        }

        [AcceptVerbs("GET", "POST")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail(string email, string? id)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                if (!string.IsNullOrEmpty(id) && user.Id == id)
                {
                    return Json(true);
                }

                return Json($"Email '{email}' đã được sử dụng.");
            }
            return Json(true);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null) 
                {
                    return RedirectToAction("ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                var callbackUrl = Url.Action("ResetPassword", "Account",
                    new { email = model.Email, code = code }, 
                    protocol: HttpContext.Request.Scheme);

                TempData["TestResetLink"] = callbackUrl;

                return RedirectToAction("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string code = null, string email = null)
        {
            if (code == null) return BadRequest("A code must be supplied for password reset.");
            var model = new ResetPasswordViewModel
            {
                Code = code,
                Email = email 
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}