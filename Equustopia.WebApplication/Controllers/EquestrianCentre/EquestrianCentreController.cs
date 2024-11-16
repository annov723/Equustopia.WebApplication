namespace Equustopia.WebApplication.Controllers.EquestrianCentre
{
    using Data;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class EquestrianCentreController : Controller
    {
        private readonly AppDbContext _context;

        public EquestrianCentreController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /EquestrianCentre/Details/{id}
        public IActionResult Details(int id)
        {
            var stable = _context.EquestrianCentres.Include(h => h.UserData).Include(h => h.Horses).FirstOrDefault(s => s.id == id);
            if (stable == null)
            {
                return NotFound("Equestrian centre not found");
            }

            return View(stable);
        }
    }
}