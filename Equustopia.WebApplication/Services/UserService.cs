namespace Equustopia.WebApplication.Services
{
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Uzytkownik>> GetUsersAsync()
        {
            return await _context.Uzytkownicy.ToListAsync();
        }
    }
}