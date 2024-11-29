﻿namespace Equustopia.WebApplication.Controllers.EquestrianCentre
{
    using Data;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
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
            var centre = _context.EquestrianCentres.Include(h => h.UserData).Include(h => h.Horses).FirstOrDefault(s => s.id == id);
            if (centre == null)
            {
                return NotFound("Equestrian centre not found");
            }
            
            int? userId = HttpContext.Session.GetInt32("UserId") ?? null;
            var isOwner = userId != null && centre.userId == userId;
            ViewBag.IsOwner = isOwner;

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
    }
}