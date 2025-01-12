namespace Equustopia.WebApplication.ViewModels
{
    using Models.Main;

    public class UserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsPrivate { get; set; }
        public List<Horse> Horses { get; set; }
        public List<EquestrianCentre> EquestrianCentres { get; set; }
    }
}