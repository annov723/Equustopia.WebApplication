﻿namespace Equustopia.WebApplication.Models.Requests
{
    public class EquestrianCentreRequest
    {
        public int Id { get; set; }
        public string? Name { get; set; } = null;
        public string? Address{ get; set; } = null;
        public double? Latitude { get; set; } = null;
        public double? Longitude { get; set; } = null;
        public int? UserId { get; set; } = null;
    }
}