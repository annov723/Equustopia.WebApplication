namespace Equustopia.WebApplication.ViewModels
{
    using Models.Main;

    public class EquestrianCentreViewModel
    {
        public required string Name { get; set; }
        public required UserData Owner { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Address { get; set; }
        public required List<Horse> Horses { get; set; }
    }
}