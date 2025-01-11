namespace Equustopia.WebApplication.Controllers
{
    using Data;
    using Microsoft.AspNetCore.Mvc;

    public class SearchController : Controller
    {
        private readonly AppDbContext _context;

        public SearchController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new { success = false, message = "Search query cannot be empty." });
            }

            var horses = _context.PublicHorses
                .Where(h => h.name.ToLower().Contains(query.ToLower()))
                .Select(h => new { Type = "Horse", Id = h.id, Name = h.name })
                .ToList();

            var stables = _context.EquestrianCentres
                .Where(s => s.name.ToLower().Contains(query.ToLower()))
                .Select(s => new { Type = "EquestrianCentre", Id = s.id, Name = s.name })
                .ToList();

            var users = _context.PublicUsers
                .Where(u => u.name.ToLower().Contains(query.ToLower()))
                .Select(u => new { Type = "User", Id = u.id, Name = u.name })
                .ToList();
            
            var results = horses.Concat(stables).Concat(users).ToList();

            if (results.Count == 0)
            {
                return NotFound(new { success = false, message = "No matching results found." });
            }

            return Ok(new { success = true, data = results });
        }
    }
}