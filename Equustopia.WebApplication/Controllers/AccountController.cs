﻿namespace Equustopia.WebApplication.Controllers
{
    using Data;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Models.Main;
    using Models.Requests;
    using Npgsql;

    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> LogIn([FromBody] LogInRequest logInRequest)
        {
            if (string.IsNullOrEmpty(logInRequest.Email) || string.IsNullOrEmpty(logInRequest.Password))
            {
                return Json(new { success = false, message = "Fields cannot be empty." });
            }
            
            // TODO: Hash password
            var user = await _context.UsersData
                .FirstOrDefaultAsync(u => u.email == logInRequest.Email && u.password == logInRequest.Password);

            if (user != null)
            {
                HttpContext.Session.SetString("LoggedUser", user.name);
                HttpContext.Session.SetInt32("UserId", user.id);

                return Json(new { success = true });
            }
        
            return Json(new { success = false, message = "Wrong credentials provided." });
        }
        
        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            HttpContext.Session.Remove("LoggedUser");
            HttpContext.Session.Remove("UserId");

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest signUpRequest)
        {
            if (string.IsNullOrEmpty(signUpRequest.Name) || string.IsNullOrEmpty(signUpRequest.Email) || string.IsNullOrEmpty(signUpRequest.Password))
            {
                return Json(new { success = false, message = "Fields cannot be empty.", constraintName = "" });
            }
            
            var user = new UserData
            {
                name = signUpRequest.Name,
                email = signUpRequest.Email,
                password = signUpRequest.Password // TODO: Hash password
            };

            try
            {
                _context.UsersData.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                if(e.InnerException is PostgresException postgresException)
                {
                    return Json(new { success = false, message = "", constraintName = postgresException.ConstraintName });
                }
                return Json(new { success = false, message = "An error occurred while creating a new user.", constraintName = "" });
            }
        
            HttpContext.Session.SetString("LoggedUser", user.name);
            HttpContext.Session.SetInt32("UserId", user.id);
            return Json(new { success = true });
        }
    }
}