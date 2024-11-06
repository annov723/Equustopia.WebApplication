namespace Equustopia.WebApplication.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("userData", Schema = "main")]
    public class UserData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string name { get; set; }
        
        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string email { get; set; }
        
        [Required]
        [StringLength(60)]
        public string password { get; set; }
        
        public ICollection<EquestrianCentre> EquestrianCentres { get; set; }
        public ICollection<Horse> Horses { get; set; }
        public ICollection<pageViews> pagesViews { get; set; }
    }
}