namespace Equustopia.WebApplication.Models.Requests
{
    using Main;

    public class AddHorseRequest
    {
        public string Name { get; set; }
        //public string Breed { get; set; }
        public DateTime? BirthDate { get; set; } = null;
        public int? HouseId { get; set; } = null;
    }
}