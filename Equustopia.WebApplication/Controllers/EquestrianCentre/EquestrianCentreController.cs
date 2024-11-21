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
            var centre = _context.EquestrianCentres.Include(h => h.UserData).Include(h => h.Horses).FirstOrDefault(s => s.id == id);
            if (centre == null)
            {
                return NotFound("Equestrian centre not found");
            }
            var isOwner = HttpContext.Session.GetInt32("UserId") != null && centre.userId == HttpContext.Session.GetInt32("UserId");
            ViewBag.IsOwner = isOwner;

            return View(centre);
        }
    }
}