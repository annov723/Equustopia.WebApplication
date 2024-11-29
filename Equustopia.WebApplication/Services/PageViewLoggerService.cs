namespace Equustopia.WebApplication.Services
{
    using Data;
    using Microsoft.Extensions.Caching.Memory;
    using Models;
    using Models.Analytics;

    public class PageViewLoggerService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        
        private const int PageViewInterval = 10;

        public PageViewLoggerService(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task LogPageViewAsync(int? userId, string pageType, int pageId, string? ipAddress)
        {
            var cacheKey = $"PageView_{userId}_{pageType}_{pageId}";
            if (_cache.TryGetValue(cacheKey, out DateTime lastLog) && (DateTime.UtcNow - lastLog).TotalSeconds < PageViewInterval)
            {
                return;
            }
            
            var pageView = new pagesViews
            {
                userId = userId,
                pageId = pageId,
                pageType = pageType,
                ipAddress = ipAddress,
                timestamp = DateTime.UtcNow
            };

            _context.PagesViews.Add(pageView);
            await _context.SaveChangesAsync();
            
            _cache.Set(cacheKey, DateTime.UtcNow, TimeSpan.FromSeconds(PageViewInterval));
        }
    }
}