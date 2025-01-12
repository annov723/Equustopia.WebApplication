namespace Equustopia.WebApplication.Models.Main
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Reference;

    [Table("centreCreateRequest", Schema = "main")]
    public class CentreCreateRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        
        [Required]
        public int centreId { get; set; }

        [Column(TypeName = "requestStatus")]
        public RequestStatus status { get; set; } = RequestStatus.New;

        public DateTime createdAt { get; set; } = DateTime.UtcNow;
        
        public DateTime updatedAt { get; set; } = DateTime.UtcNow;
        
        public required EquestrianCentre EquestrianCentre { get; set; }
    }
}