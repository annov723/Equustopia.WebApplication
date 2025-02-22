﻿namespace Equustopia.WebApplication.Models.Requests
{
    public class UserRequest
    {
        public int Id { get; set; }
        public string? Name { get; set; } = null;
        public string? Email { get; set; } = null;
        public string? Password { get; set; } = null;
    }
}