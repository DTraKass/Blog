using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Blog.Models;
using System.Security.Claims;

namespace Blog.Controllers
{
    public class AccountController : Controller
    {
        private readonly LoggerService _loggerService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _loggerService = new LoggerService();
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet("account/register")]
        public IActionResult RegisterView()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost("account/register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Address = model.Address
                    };
                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        // Перенаправление на страницу профиля после успешной регистрации
                        return RedirectToAction("Profile", "Account");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

                return View("RegisterView", model);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex.Message);
                return RedirectToAction("Error", "Errors");
            }
        }

        [HttpGet("account/loginview")]
        public IActionResult LoginView()
        {
            return View();
        }

        [HttpPost("account/login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _loggerService.LogUserAction($"Пользователь {model.Email} вошел в аккаунт");

                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        return RedirectToAction("Profile", "Account", new { id = user.Id });
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Неправильный логин или пароль");
                    _loggerService.LogError($"Пользователь {model.Email} ввел неправильный логин или пароль");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.GetUserAsync(User);
            await _signInManager.SignOutAsync();

            _loggerService.LogUserAction($"Пользователь {user.FirstName} вышел из аккаунта");

            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (!User.Identity.IsAuthenticated)
            {
                _loggerService.LogError($"Кто-то пытался войти в профиль или зарегестрироваться");
                return RedirectToAction("Loginview", "Account");
            }
            return View("Profile", user); // Передаем пользователя в представление
        }
    }
}
