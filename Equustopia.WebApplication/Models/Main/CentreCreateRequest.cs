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
        public int Id { get; set; }

        [Column(TypeName = "requestStatus")]
        public RequestStatus Status { get; set; } = RequestStatus.New;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}