namespace Equustopia.WebApplication.Models.Main
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("approvedEquestrianCentres", Schema = "main")]
    public class ApprovedEquestrianCentres
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        
        [Required]
        [StringLength(250, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 250 characters.")]
        public required string name { get; set; }
        
        [Required]
        public required int userId { get; set; }
        
        public double? latitude { get; set; }
        public double? longitude { get; set; }

        [StringLength(250, MinimumLength = 2, ErrorMessage = "Address must be between 2 and 250 characters.")]
        public string? address { get; set; }
        
        [Column(TypeName = "jsonb")]
        public string? contactInformation { get; set; }
        
        [Column(TypeName = "jsonb")]
        public string? openHours { get; set; }
    }
}