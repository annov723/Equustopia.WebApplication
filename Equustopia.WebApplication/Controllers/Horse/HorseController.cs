namespace Equustopia.WebApplication.Controllers.Horse
{
    using System.Security.Claims;
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
            
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool isOwner = currentUserId != null && int.Parse(currentUserId) == horse.userId;
            ViewBag.IsOwner = isOwner;

            return View(horse);
        }
    }
}