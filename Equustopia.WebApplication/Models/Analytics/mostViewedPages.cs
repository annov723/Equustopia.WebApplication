namespace Equustopia.WebApplication.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("NajczesciejWyswietlaneStrony", Schema = "analytics")]
    public class mostViewedPages
    {
        public int pageId { get; set; }
    
        [Column(TypeName = "varchar(50)")]
        public string pageType { get; set; }
    
        public int viewsCount { get; set; }
    
        public int position { get; set; }
    }
}