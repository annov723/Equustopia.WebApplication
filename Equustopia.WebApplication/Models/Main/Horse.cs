namespace Equustopia.WebApplication.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Main;

    [Table("horse", Schema = "main")]
    public class Horse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Username must be between 2 and 50 characters.")]
        public required string name { get; set; }

        [Required]
        public required int userId { get; set; }

        public int? centreId { get; set; }
        
        public DateTime? birthDate { get; set; }
        
        public required UserData UserData { get; set; }
        public EquestrianCentre? EquestrianCentre { get; set; }
    }
}