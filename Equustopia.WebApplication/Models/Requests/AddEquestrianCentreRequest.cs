namespace Equustopia.WebApplication.Models.Requests
{
    public class AddEquestrianCentreRequest
    {
        public string Name { get; set; }
        public string? Address{ get; set; } = null;
    }
}