namespace Equustopia.WebApplication.Controllers
{
    using Data;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Models.Requests;
    using Npgsql;
    using Services;

    public class HorseController : Controller
    {
        private readonly PageViewLoggerService _pageViewLogger;
        private readonly AppDbContext _context;

        public HorseController(AppDbContext context, PageViewLoggerService pageViewLogger)
        {
            _context = context;
            _pageViewLogger = pageViewLogger;
        }
        
        // GET: /Horse/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var horse = _context.Horses.Include(h => h.UserData).Include(h => h.EquestrianCentre).FirstOrDefault(h => h.id == id);
            if (horse == null)
            {
                return NotFound("Horse not found");
            }
            
            int? userId = HttpContext.Session.GetInt32("UserId") ?? null;
            var isOwner = userId != null && horse.userId == userId;
            ViewBag.IsOwner = isOwner;

            if (!isOwner)
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                await _pageViewLogger.LogPageViewAsync(userId, "horse", id, ipAddress);
            }

            return View(horse);
        }
        
        // POST: /Horse/Edit/{id}
        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] HorseRequest? horseData)
        {
            if (horseData == null)
            {
                return Json(new { success = false, message = "Horse data is missing." });
            }

            var horse = await _context.Horses.FirstOrDefaultAsync(h => h.id == horseData.Id);
            if (horse == null)
            {
                return Json(new { success = false, message = "Horse not found." });
            }

            horse.name = horseData.Name;
            horse.birthDate = horseData.BirthDate;
            horse.centreId = horseData.EquestrianCentreId;

            try
            {
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                if(e.InnerException is PostgresException postgresException)
                {
                    return Json(new { success = false, message = "", constraintName = postgresException.ConstraintName });
                }
                return Json(new { success = false, message = "An error occurred while updating the horse. " + e.Message });
            }
        }
        
        // POST: /Horse/Remove/{id}
        [HttpPost]
        public IActionResult Remove([FromBody] HorseRequest? request)
        {
            if (request == null || request.Id <= 0)
            {
                return Json(new { success = false, message = "Invalid horse id." });
            }

            var horse = _context.Horses.FirstOrDefault(h => h.id == request.Id);

            if (horse == null)
            {
                return Json(new { success = false, message = "Horse not found." });
            }
            
            _context.Horses.Remove(horse);
            _context.SaveChanges();

            return Json(new { success = true });
        }
        
        // POST: /Horse/PrivacyChange
        [HttpPost]
        public async Task<IActionResult> PrivacyChange([FromBody] HorseRequest? request)
        {
            if (request == null || request.Id <= 0)
            {
                return Json(new { success = false, message = "Invalid horse id." });
            }
            
            try
            {
                var horse = await _context.Horses.FindAsync(request.Id);
                if (horse == null)
                {
                    return Json(new { success = false, message = "Horse not found." });
                }

                horse.isPrivate = !horse.isPrivate;
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while changing horse privacy setting." + ex });
            }
        }
    }
}