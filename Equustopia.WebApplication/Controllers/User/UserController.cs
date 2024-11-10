namespace Equustopia.WebApplication.Controllers.User
{
    using Data;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels;

    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }
        
        public IActionResult UserMainPage()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("LogOut", "Account");
            }

            var userData = _context.UsersData.FirstOrDefault(u => u.id == userId);
            if (userData == null)
            {
                return RedirectToAction("LogOut", "Account");
            }

            var userViewModel = new UserViewModel
            {
                Name = userData.name,
                Email = userData.email
            };
            
            return View(userViewModel);
        }
    }
}