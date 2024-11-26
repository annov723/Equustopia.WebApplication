namespace Equustopia.WebApplication.ViewModels
{
    using Models.Main;

    public class EquestrianCentreViewModel
    {
        public string Name { get; set; }
        public UserData Owner { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public string? address { get; set; }
        public List<Horse> Horses { get; set; }
    }
}