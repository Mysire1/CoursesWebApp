using CoursesWebApp.Models;
using CoursesWebApp.Models.ViewModels;
using CoursesWebApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CoursesWebApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _authService.ValidateUserAsync(model.Username, model.Password);
            if (result == null)
            {
                ModelState.AddModelError("", "Некоректний email або пароль");
                return View(model);
            }
            var (user, role) = result.Value;
            int userId;
            string fullName;
            string email;
            if (role == "Student" && user is Student student)
            {
                userId = student.StudentId;
                fullName = student.FullName;
                email = student.Email;
            }
            else if (role == "Teacher" && user is Teacher teacher)
            {
                userId = teacher.TeacherId;
                fullName = teacher.FullName;
                email = teacher.Email;
            }
            else
            {
                ModelState.AddModelError("", "Помилка авторизації");
                return View(model);
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
                new Claim("FullName", fullName)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(8)
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);
            _logger.LogInformation("Користувач {Email} ({Role}) успішно авторизувався", email, role);
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                if (!await _authService.IsEmailAvailableAsync(model.Email))
                {
                    ModelState.AddModelError("Email", "Цей email вже зареєстровано");
                    return View(model);
                }
                var result = await _authService.RegisterUserAsync(model);
                if (result == null)
                {
                    ModelState.AddModelError("", "Помилка при реєстрації (невдалий запис у базу).");
                    return View(model);
                }
                var (user, role) = result.Value;
                _logger.LogInformation("Новий користувач зареєстрован: {Email} ({Role})", model.Email, role);
                TempData["SuccessMessage"] = $"Реєстрація успішна! Тепер ви можете увійти в систему";
                return RedirectToAction("Login");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Помилка при реєстрації: {Message}", ex.Message);
                ModelState.AddModelError("", $"Сталася системна помилка: {ex.Message}");
                return View(model);
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("Користувач {Email} вийшов з системи", email);
            TempData["SuccessMessage"] = "Ви успішно вийшли з системи";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(role))
            {
                return NotFound();
            }
            var user = await _authService.GetUserByIdAsync(userId, role);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["Role"] = role;
            return View(user);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
