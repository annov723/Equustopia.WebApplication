namespace Equustopia.WebApplication.Controllers.Horse
{
    using Data;
    using Microsoft.AspNetCore.Mvc;

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
            var horse = _context.Horses.FirstOrDefault(h => h.id == id);
            if (horse == null)
            {
                return NotFound("Horse not found");
            }

            return View(horse);
        }
    }
}