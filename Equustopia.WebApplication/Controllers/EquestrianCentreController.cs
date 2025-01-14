namespace Equustopia.WebApplication.Controllers
{
    using Data;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Models.Helpers;
    using Models.Main;
    using Models.Requests;
    using Npgsql;
    using Services;

    public class EquestrianCentreController : Controller
    {
        private readonly PageViewLoggerService _pageViewLogger;
        private readonly AppDbContext _context;

        public EquestrianCentreController(AppDbContext context, PageViewLoggerService pageViewLogger)
        {
            _context = context;
            _pageViewLogger = pageViewLogger;
        }

        // GET: /EquestrianCentre/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var centre = _context.EquestrianCentres.Include(h => h.UserData).
                Include(h => h.Horses)!.ThenInclude(h => h.UserData).FirstOrDefault(s => s.id == id);
            if (centre == null)
            {
                return NotFound("Equestrian centre not found");
            }
            
            int? userId = HttpContext.Session.GetInt32("UserId") ?? null;
            var isOwner = userId != null && centre.userId == userId;
            ViewBag.IsOwner = isOwner;
            if (userId != null)
            {
                ViewBag.UserId = userId;
            }
            else
            {
                ViewBag.userId = -1;
            }
            

            if (!isOwner)
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                await _pageViewLogger.LogPageViewAsync(userId, "equestrianCentre", id, ipAddress);
            }

            return View(centre);
        }
        
        // POST: /EquestrianCentre/Edit/{id}
        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] EquestrianCentreRequest? centreData)
        {
            if (centreData == null)
            {
                return Json(new { success = false, message = "Centre data is missing." });
            }
            
            if (!string.IsNullOrEmpty(centreData.Address) && (centreData.Address.Length > 250 || centreData.Address.Length < 2))
            {
                return Json(new { success = false, message = "", constraintName = "chk_address_length" });
            }

            var centre = await _context.EquestrianCentres.FirstOrDefaultAsync(h => h.id == centreData.Id);
            if (centre == null)
            {
                return Json(new { success = false, message = "Centre not found." });
            }

            centre.name = centreData.Name;
            centre.address = centreData.Address;
            centre.longitude = centreData.Longitude;
            centre.latitude = centreData.Latitude;

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
                return Json(new { success = false, message = "An error occurred while updating the centre. " + e.Message });
            }
        }
        
        // POST: /EquestrianCentre/Remove/{id}
        [HttpPost]
        public IActionResult Remove([FromBody] EquestrianCentreRequest? request)
        {
            if (request == null || request.Id <= 0)
            {
                return Json(new { success = false, message = "Invalid centre id." });
            }

            var centre = _context.EquestrianCentres.FirstOrDefault(h => h.id == request.Id);

            if (centre == null)
            {
                return Json(new { success = false, message = "Centre not found." });
            }
            
            _context.EquestrianCentres.Remove(centre);
            _context.SaveChanges();

            return Json(new { success = true });
        }
        
        [HttpGet]
        public async Task<IActionResult> GetCentreViews(int centreId, DateTime startDate, DateTime endDate)
        {
            var centreViewsByDate = await _context.Set<CentreViewsByDate>()
                .FromSqlRaw(
                    @"SELECT * FROM analytics.get_centre_views_by_date({0}, {1}, {2})", 
                    startDate.Date, endDate.Date, centreId)
                .ToListAsync();
        
            if (centreViewsByDate.Count == 0) return NotFound();

            return Json(centreViewsByDate);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetHourlyViews(int centreId)
        {
            var hourlyViews = await _context.Set<CentreViewsByDate>() // Assume HourlyViews is a model class
                .FromSqlRaw("SELECT * FROM analytics.get_centre_views_by_hour({0})", centreId)
                .ToListAsync();

            if (hourlyViews.Count == 0) return NotFound();

            return Json(hourlyViews);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetHorsesAgeGroups(int centreId)
        {
            var horseAgeGroups = await _context.Set<HorseAgeGroup>()
                .FromSqlRaw(
                    @"SELECT * FROM main.get_horse_counts_by_age_group({0})", 
                    centreId)
                .ToListAsync();
        
            if (horseAgeGroups.Count == 0) return NotFound();
            
            var result = new
            {
                ageGroup_0_3 = horseAgeGroups.FirstOrDefault(x => x.age_group == "0_3")?.horse_count ?? 0,
                ageGroup_3_10 = horseAgeGroups.FirstOrDefault(x => x.age_group == "3_10")?.horse_count ?? 0,
                ageGroup_10_19 = horseAgeGroups.FirstOrDefault(x => x.age_group == "10_19")?.horse_count ?? 0,
                ageGroup_19_Plus = horseAgeGroups.FirstOrDefault(x => x.age_group == "19_")?.horse_count ?? 0,
            };
            
            return Json(result);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetHorsesBreedGroups(int centreId)
        {
            var horseBreedGroups = await _context.Set<HorseBreedGroup>()
                .FromSqlRaw(
                    @"SELECT * FROM main.get_horse_count_by_breed({0})", 
                    centreId)
                .ToListAsync();
        
            if (horseBreedGroups.Count == 0) return NotFound();
            
            return Json(horseBreedGroups);
        }
        
        
        
        [HttpPost]
        public async Task<IActionResult> CreateNewCentreRequest([FromBody] int centreId)
        {
            var centre = await _context.EquestrianCentres.FindAsync(centreId);
            if (centre == null)
            {
                return Json(new { success = false, message = "Centre not found." });
            }

            var newRequest = new CentreCreateRequest
            {
                centreId = centreId,
                status = (int)RequestStatus.New,
                createdAt = DateTime.UtcNow,
                updatedAt = DateTime.UtcNow,
                EquestrianCentre = centre
            };

            _context.CentreCreateRequests.Add(newRequest);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true, 
                newRequest.id,
                centreName = centre.name,
                status = newRequest.status.ToString(),
                createdAt = newRequest.createdAt.ToString("dd/MM/yyyy HH:mm"),
                updatedAt = newRequest.updatedAt.ToString("dd/MM/yyyy HH:mm")
            });
        }
        
        [HttpGet]
        public async Task<IActionResult> GetCentreRequestView(int centreId)
        {
            var centre = _context.EquestrianCentres.FirstOrDefault(c => c.id == centreId);
            if (centre == null)
            {
                return Json(new { success = false, message = "Centre not found." });
            }

            var existingRequest = _context.CentreCreateRequests
                .Where(r => r.centreId == centreId)
                .OrderByDescending(r => r.createdAt)
                .FirstOrDefault();
            
            if (existingRequest == null)
            {
                return Json(new
                {
                    success = true,
                    id = 0
                });
            }

            return Json(new
            {
                success = true,
                existingRequest.id,
                centreName = centre.name,
                status = ((RequestStatus)existingRequest.status).ToString(),
                createdAt = existingRequest.createdAt.ToString("dd/MM/yyyy HH:mm"),
                updatedAt = existingRequest.updatedAt.ToString("dd/MM/yyyy HH:mm")
            });
        }
        
        [HttpPost]
        public async Task<IActionResult> UpdateRequestStatus([FromBody] UpdateRequestStatusRequest request)
        {
            var centreRequest = await _context.CentreCreateRequests.FindAsync(request.RequestId);
            if (centreRequest == null)
            {
                return Json(new { success = false, message = "Request not found." });
            }

            try
            {
                centreRequest.status = (int)request.Status;
                centreRequest.updatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}