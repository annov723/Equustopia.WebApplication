namespace Equustopia.WebApplication.Models.Main
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Attributes;

    [Table("publicHorses", Schema = "main")]
    public class PublicHorses
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters.")]
        public required string name { get; set; }

        [Required]
        public required int userId { get; set; }
        
        public int? centreId { get; set; }

        [DateNotInFuture(ErrorMessage = "Birth date cannot be in the future.")]
        public DateTime? birthDate { get; set; }
        
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Breed must be between 2 and 100 characters.")]
        public string? breed { get; set; }
        
        [StringLength(1000, MinimumLength = 2, ErrorMessage = "Breed must be between 2 and 1000 characters.")]
        public string? photo { get; set; }
        
        [NumberPositive(ErrorMessage = "Height must be grater than 0.")]
        public double? height { get; set; }

        public required UserData UserData { get; set; }
        public EquestrianCentre? EquestrianCentre { get; set; }
        
        [NotMapped]
        public bool IsOwnerLogged { get; set; }
    }
}