namespace Equustopia.WebApplication.Models.Main
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Main;

    [Table("equestrianCentre", Schema = "main")]
    public class EquestrianCentre
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
        
        public required UserData UserData { get; set; }
        public ICollection<Horse> Horses { get; set; }
        
        
    }
}