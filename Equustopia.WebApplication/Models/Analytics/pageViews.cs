namespace Equustopia.WebApplication.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("pageViews", Schema = "analytics")]
    public class pageViews
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int? userId { get; set; }

        [Required]
        public int pageId { get; set; }

        [Required]
        [MaxLength(50)]
        public string pageType { get; set; }

        public DateTime timestamp { get; set; } = DateTime.Now;

        [MaxLength(45)]
        public string ipAddress { get; set; }
        
        public UserData UserData { get; set; }
    }
}