﻿namespace Equustopia.WebApplication.Services
{
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Models.Main;

    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserData>> GetUsersAsync()
        {
            return await _context.UsersData.ToListAsync();
        }
    }
}