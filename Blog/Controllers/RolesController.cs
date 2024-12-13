using Blog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly LoggerService _loggerService;

        public RolesController(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;

            _loggerService = new LoggerService();
        }

        public async Task<IActionResult> AddRole(string userId, string roleName)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }

                var result = await _userManager.AddToRoleAsync(user, roleName);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                _loggerService.LogUserAction($"Пользователь {user.FirstName} создал роль ");
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex.Message);
            }
            return View();
        }
    }
}
