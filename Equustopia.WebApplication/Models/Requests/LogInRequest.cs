namespace Equustopia.WebApplication.Models.Requests
{
    public class LogInRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}