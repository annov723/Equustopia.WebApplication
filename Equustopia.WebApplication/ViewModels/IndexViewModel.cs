namespace Equustopia.WebApplication.ViewModels
{
    using Helpers;
    using Models.Main;

    public class IndexViewModel
    {
        public List<ListWithTypesHelper>? MostViewedPages { get; set; }
        public bool IsModerator { get; set; }
        public List<CentreCreateRequest>? Requests { get; set; }
    }
}