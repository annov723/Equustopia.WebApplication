namespace Equustopia.WebApplication.Models.Main
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Attributes;

    [Table("publicUsers", Schema = "main")]
    public class PublicUsers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Username must be between 2 and 50 characters.")]
        public required string name { get; set; }
        
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(255, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 255 characters.")]
        public required string email { get; set; }
        
        
        [DateNotInFuture(ErrorMessage = "Birth date cannot be in the future.")]
        public DateTime? birthDate { get; set; }
        
        [StringLength(1000, MinimumLength = 2, ErrorMessage = "Photo path must be between 2 and 1000 characters.")]
        public string? profilePhoto { get; set; }
    }
}