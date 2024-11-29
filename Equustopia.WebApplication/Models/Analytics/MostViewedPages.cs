namespace Equustopia.WebApplication.Models.Analytics
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("mostViewedPages", Schema = "analytics")]
    public class MostViewedPages
    {
        [Key]
        public int pageId { get; set; }
    
        [Required]
        [StringLength(2, MinimumLength = 50, ErrorMessage = "PageType must be between 2 and 50 characters.")]
        public string pageType { get; set; }
    
        public int viewsCount { get; set; }
    
        public int position { get; set; }
    }
}