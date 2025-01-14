namespace Equustopia.WebApplication.Controllers
{
    using Data;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using ViewModels;
    using ViewModels.Helpers;

    public class IndexController : Controller
    {
        private readonly AppDbContext _context;

        public IndexController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var results = await (
                from mv in _context.MostViewedPages
                join h in _context.Horses on mv.pageId equals h.id into horseGroup
                from hg in horseGroup.DefaultIfEmpty()
                join ec in _context.EquestrianCentres on mv.pageId equals ec.id into centreGroup
                from cg in centreGroup.DefaultIfEmpty()
                where (mv.pageType == "horse" && hg != null) || (mv.pageType == "equestrianCentre" && cg != null)
                select new ListWithTypesHelper
                {
                    Type = mv.pageType == "horse" ? "Horse" : "EquestrianCentre",
                    Id = mv.pageType == "horse" ? hg.id : cg.id,
                    Name = mv.pageType == "horse" ? hg.name : cg.name
                }
            ).ToListAsync();
            
            var userId = HttpContext.Session.GetInt32("UserId");
            var isModerator = userId != null && await _context.UsersData
                .AnyAsync(u => u.id == userId && u.moderator);
            var requests = isModerator ? await _context.CentreCreateRequests
                    .Include(r => r.EquestrianCentre).ToListAsync() : null;
            
            var viewModel = new IndexViewModel
            {
                MostViewedPages = results,
                IsModerator = isModerator,
                Requests = requests
            };
            
            return View(viewModel);
        }
    }
}