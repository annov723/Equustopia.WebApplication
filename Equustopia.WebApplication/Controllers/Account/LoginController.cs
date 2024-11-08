namespace Equustopia.WebApplication.Controllers.Account
{
    using Data;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Models.Requests;

    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return Json(new { success = false, message = "Email and password cannot be empty." });
            }
            
            var user = await _context.UsersData
                .FirstOrDefaultAsync(u => u.email == loginRequest.Email && u.password == loginRequest.Password);

            if (user != null)
            {
                HttpContext.Session.SetString("LoggedUser", user.name);
                HttpContext.Session.SetInt32("UserId", user.id);

                return Json(new { success = true });
            }
        
            return Json(new { success = false, message = "Wrong credentials provided." });
        }
        
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("LoggedUser");
            HttpContext.Session.Remove("UserId");

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Signup()
        {
            
            return Json(new { success = true });
        }
    }
}