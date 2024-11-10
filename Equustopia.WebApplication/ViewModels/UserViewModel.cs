namespace Equustopia.WebApplication.ViewModels
{
    using Models;

    public class UserViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public List<Horse> Horses { get; set; }
    }
}