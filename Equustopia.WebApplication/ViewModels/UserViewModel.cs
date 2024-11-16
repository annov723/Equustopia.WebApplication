namespace Equustopia.WebApplication.ViewModels
{
    using Models;
    using Models.Main;

    public class UserViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public List<Horse> Horses { get; set; }
        public List<EquestrianCentre> EquestrianCentres { get; set; }
    }
}