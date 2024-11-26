namespace Equustopia.WebApplication.Controllers.Horse
{
    using Data;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Models.Requests;
    using Npgsql;

    public class HorseController : Controller
    {
        private readonly AppDbContext _context;

        public HorseController(AppDbContext context)
        {
            _context = context;
        }
        
        // GET: /Horse/Details/{id}
        public IActionResult Details(int id)
        {
            var horse = _context.Horses.Include(h => h.UserData).Include(h => h.EquestrianCentre).FirstOrDefault(h => h.id == id);
            if (horse == null)
            {
                return NotFound("Horse not found");
            }
            
            var isOwner = HttpContext.Session.GetInt32("UserId") != null && horse.userId == HttpContext.Session.GetInt32("UserId");
            ViewBag.IsOwner = isOwner;

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
    }
}