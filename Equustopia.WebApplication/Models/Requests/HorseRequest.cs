namespace Equustopia.WebApplication.Models.Requests
{
    public class HorseRequest
    {
        public int Id { get; set; }
        public string? Name { get; set; } = null;
        public string? Breed { get; set; } = null;
        public DateTime? BirthDate { get; set; } = null;
        public int? EquestrianCentreId { get; set; } = null;
    }
}