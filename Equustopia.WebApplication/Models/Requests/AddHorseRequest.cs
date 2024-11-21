namespace Equustopia.WebApplication.Models.Requests
{
    public class AddHorseRequest
    {
        public string Name { get; set; }
        //public string Breed { get; set; }
        public DateTime? BirthDate { get; set; } = null;
        public int? EquestrianCentreId { get; set; } = null;
    }
}