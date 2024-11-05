namespace Equustopia.WebApplication.Controllers.Account
{
    using Data;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Retrieve user with matching email
            var user = await _context.Uzytkownicy
                .FirstOrDefaultAsync(u => u.email == email && u.haslo == password);

            if (user != null)
            {
                // Login success logic - set session or authentication
                HttpContext.Session.SetString("LoggedUser", user.id.ToString());
                HttpContext.Session.SetInt32("UserId", user.id);

                // Hide the login modal and return a success response
                // return Json(new { success = true });
                return RedirectToAction("Index", "Home");
            }
        
            // Invalid credentials
            // return Json(new { success = false, message = "Invalid email or password." });
            TempData["LoginError"] = "Invalid email or password.";
            return RedirectToAction("Index", "Home"); 
        }
    }
}