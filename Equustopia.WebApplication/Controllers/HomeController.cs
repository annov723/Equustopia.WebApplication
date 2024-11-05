using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Equustopia.WebApplication.Models;

namespace Equustopia.WebApplication.Controllers
{
    using Services;

    public class HomeController : Controller
    {
        private readonly UserService _userService;

        public HomeController(UserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetUsersAsync();
            return View(users);
        }
    }
}