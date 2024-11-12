namespace Equustopia.WebApplication.Controllers.EquestrianCentre
{
    using Data;
    using Microsoft.AspNetCore.Mvc;

    public class EquestrianCentreController : Controller
    {
        private readonly AppDbContext _context;

        public EquestrianCentreController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Stable/Details/{id}
        public IActionResult Details(int id)
        {
            var stable = _context.EquestrianCentres.FirstOrDefault(s => s.id == id);
            if (stable == null)
            {
                return NotFound("Stable not found");
            }

            return View(stable);
        }
    }
}