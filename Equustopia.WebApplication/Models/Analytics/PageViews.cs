namespace Equustopia.WebApplication.Models.Analytics
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Equustopia.WebApplication.Models.Main;

    [Table("pagesViews", Schema = "analytics")]
    public class pagesViews
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int? userId { get; set; }

        [Required]
        public int pageId { get; set; }

        [Required]
        [StringLength(2, MinimumLength = 50, ErrorMessage = "PageType must be between 2 and 50 characters.")]
        public string pageType { get; set; }

        public DateTime timestamp { get; set; } = DateTime.Now;

        [MaxLength(45)]
        [StringLength(2, MinimumLength = 45, ErrorMessage = "IpAddress must be between 2 and 45 characters.")]
        public string? ipAddress { get; set; }
        
        public UserData? UserData { get; set; }
    }
}