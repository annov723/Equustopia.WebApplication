namespace Equustopia.WebApplication.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using Models;
    using Models.Main;

    public class HorseViewModel
    {
        public string Name { get; set; }
        //public string Breed { get; set; }
        public DateTime BirthDate { get; set; }
        public UserData Owner { get; set; }
        public EquestrianCentre House { get; set; }
    }
}