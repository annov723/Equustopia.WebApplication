namespace Equustopia.WebApplication.Controllers.Horse
{
    using Data;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

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
            var horse = _context.Horses.Include(h => h.UserData).FirstOrDefault(h => h.id == id);
            if (horse == null)
            {
                return NotFound("Horse not found");
            }
            
            var isOwner = HttpContext.Session.GetInt32("UserId") != null && horse.userId == HttpContext.Session.GetInt32("UserId");
            ViewBag.IsOwner = isOwner;

            return View(horse);
        }
        
        // POST: /Horse/Edit/{id}
        public IActionResult EditHorse(int id)
        {
            var horse = _context.Horses.Include(h => h.UserData).FirstOrDefault(h => h.id == id);
            if (horse == null)
            {
                return NotFound();
            }

            // TO DO!!!

            return View(horse);
        }
    }
}