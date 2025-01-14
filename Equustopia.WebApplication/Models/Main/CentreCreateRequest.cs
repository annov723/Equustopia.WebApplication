namespace Equustopia.WebApplication.Models.Main
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Helpers;

    [Table("centreCreateRequest", Schema = "main")]
    public class CentreCreateRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        
        [Required]
        public int centreId { get; set; }

        [Required]
        public int status { get; set; } = (int)RequestStatus.New;

        public DateTime createdAt { get; set; } = DateTime.UtcNow;
        
        public DateTime updatedAt { get; set; } = DateTime.UtcNow;
        
        public required EquestrianCentre EquestrianCentre { get; set; }
    }
}