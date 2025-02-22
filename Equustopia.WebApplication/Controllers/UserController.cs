﻿namespace Equustopia.WebApplication.Controllers
{
    using Data;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Models.Main;
    using Models.Requests;
    using Npgsql;
    using ViewModels;

    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }
        
        // GET: /User/Details/{id}
        public IActionResult Details(int id)
        {
            var user = _context.UsersData.Include(h => h.Horses).Include(h => h.EquestrianCentres).FirstOrDefault(u => u.id == id);
            if (user == null)
            {
                return NotFound("User not found");
            }
            
            if(id == HttpContext.Session.GetInt32("UserId"))
            {
                return RedirectToAction("UserMainPage");
            }

            return View(user);
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
            
            var userHorses = _context.Horses.Where(h => h.userId == userId).ToList();
            var userCentres = _context.EquestrianCentres.Where(h => h.userId == userId).
                ToList();

            var userViewModel = new UserViewModel
            {
                Id = userData.id,
                Name = userData.name,
                Email = userData.email,
                IsPrivate = userData.isPrivate,
                Horses = userHorses,
                EquestrianCentres = userCentres
            };
            
            return View(userViewModel);
        }
       
        [HttpPost]
        public async Task<IActionResult> AddHorse([FromBody] HorseRequest? addHorseRequest)
        {
            if (addHorseRequest == null)
            {
                return Json(new { success = false, message = "An error occurred while creating a new horse.", 
                    constraintName = "" });
            }
            
            if (string.IsNullOrEmpty(addHorseRequest.Name))
            {
                return Json(new { success = false, message = "Name cannot be empty.", constraintName = "" });
            }
            
            var owner = _context.UsersData.FirstOrDefault(u => u.id == HttpContext.Session.GetInt32("UserId")!.Value);
            if(owner == null)
            {
                return Json(new { success = false, message = "Owner does not exist.", constraintName = "" });
            }

            var centre = addHorseRequest.EquestrianCentreId == null ? null : _context.EquestrianCentres.FirstOrDefault(
                c => c.id == addHorseRequest.EquestrianCentreId);
            
            var horse = new Horse
            {
                name = addHorseRequest.Name,
                breed = addHorseRequest.Breed,
                birthDate = addHorseRequest.BirthDate,
                userId = owner.id,
                centreId = addHorseRequest.EquestrianCentreId,
                UserData = owner,
                EquestrianCentre = centre
            };
            
            try
            {
                _context.Horses.Add(horse);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                if(e.InnerException is PostgresException postgresException)
                {
                    return Json(new { success = false, message = "", constraintName = postgresException.ConstraintName });
                }
                return Json(new { success = false, message = "An error occurred while creating a new horse.", constraintName = "" });
            }
            
            return Json(new { success = true });
        }
        
        [HttpPost]
        public async Task<IActionResult> AddEquestrianCentre([FromBody] EquestrianCentreRequest equestrianCentreRequest)
        {
            if (string.IsNullOrEmpty(equestrianCentreRequest.Name))
            {
                return Json(new { success = false, message = "Name cannot be empty.", constraintName = "" });
            }

            if (string.IsNullOrEmpty(equestrianCentreRequest.Address))
            {
                equestrianCentreRequest.Address = null;
            }
            else if (equestrianCentreRequest.Address.Length > 250 || equestrianCentreRequest.Address.Length < 2)
            {
                return Json(new { success = false, message = "", constraintName = "chk_address_length" });
            }
            
            var owner = _context.UsersData.FirstOrDefault(u => u.id == HttpContext.Session.GetInt32("UserId")!.Value);
            if(owner == null)
            {
                return Json(new { success = false, message = "Owner does not exist.", constraintName = "" });
            }

            var centre = new EquestrianCentre
            {
                name = equestrianCentreRequest.Name,
                address = equestrianCentreRequest.Address,
                userId = owner.id,
                UserData = owner
            };
            
            try
            {
                _context.EquestrianCentres.Add(centre);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                if(e.InnerException is PostgresException postgresException)
                {
                    return Json(new { success = false, message = "", constraintName = postgresException.ConstraintName });
                }
                return Json(new { success = false, message = "An error occurred while creating a new equestrian centre.", 
                    constraintName = "" });
            }
            
            return Json(new { success = true });
        }
        
        [HttpGet]
        public IActionResult GetApprovedEquestrianCentres()
        {
            var centres = _context.ApprovedEquestrianCentres
                .Select(s => new { s.id, s.name })
                .ToList();

            return Ok(centres);
        }
        
        [HttpPost]
        public async Task<IActionResult> Remove([FromBody] UserRequest? request)
        {
            if (request == null || request.Id <= 0)
            {
                return Json(new { success = false, message = "Invalid user id " + request?.Id + "." });
            }

            try
            {
                var user = await _context.UsersData.FindAsync(request.Id);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found." });
                }

                _context.UsersData.Remove(user);
                await _context.SaveChangesAsync();
                
                HttpContext.Session.Clear();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while removing the user. " + ex });
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> PrivacyChange([FromBody] UserRequest? request)
        {
            if (request == null || request.Id <= 0)
            {
                return Json(new { success = false, message = "Invalid user id " + request?.Id + "." });
            }
            
            try
            {
                var user = await _context.UsersData.FindAsync(request.Id);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found." });
                }

                user.isPrivate = !user.isPrivate;
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while changing user privacy setting." + ex });
            }
        }
    }
}