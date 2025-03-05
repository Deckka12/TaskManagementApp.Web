using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TaskManagementApp.Application.Interfaces;
using TaskManagementApp.Application.DTOs;

namespace TaskManagementApp.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(UserDTO userDto)
        {
            if (ModelState.IsValid)
            {
                await _userService.CreateUserAsync(userDto);
                return RedirectToAction("Index", "Home");
            }
            return View(userDto);
        }
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginDto loginDto)
        {
            if (ModelState.IsValid)
            {
                var success = await _userService.LoginAsync(loginDto.Email, loginDto.Password);
                if (success)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Неверный email или пароль.");
            }
            return View(loginDto);
        }

        public async Task<IActionResult> Logout()
        {
            await _userService.LogoutAsync();
            return RedirectToAction("Login");
        }
    }
}
