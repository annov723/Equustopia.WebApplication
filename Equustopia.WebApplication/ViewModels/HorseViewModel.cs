namespace Equustopia.WebApplication.ViewModels
{
    using Models.Main;

    public class HorseViewModel
    {
        public required string Name { get; set; }
        public string Breed { get; set; }
        public bool IsPrivate { get; set; }
        public DateTime BirthDate { get; set; }
        public UserData? Owner { get; set; }
        public EquestrianCentre? House { get; set; }
    }
}