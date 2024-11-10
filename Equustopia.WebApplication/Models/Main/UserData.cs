namespace Equustopia.WebApplication.Models.Main
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("userData", Schema = "main")]
    public class UserData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        
        [Required] // TODO: Add unique constraint to all possible errors and implement it in the controller
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Username must be between 2 and 50 characters.")]
        public required string name { get; set; }
        
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(255, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 255 characters.")]
        public required string email { get; set; }
        
        [Required]
        [StringLength(60, MinimumLength = 4, ErrorMessage = "Password must be between 4 and 60 characters.")]
        public required string password { get; set; }
        
        public ICollection<EquestrianCentre> EquestrianCentres { get; set; }
        public ICollection<Horse> Horses { get; set; }
        public ICollection<PageViews> pagesViews { get; set; }
    }
}