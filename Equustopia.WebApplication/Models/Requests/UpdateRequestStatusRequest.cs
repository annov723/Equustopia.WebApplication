namespace Equustopia.WebApplication.Models.Requests
{
    using Helpers;

    public class UpdateRequestStatusRequest
    {
        public int RequestId { get; set; }
        public RequestStatus Status { get; set; }
    }
}