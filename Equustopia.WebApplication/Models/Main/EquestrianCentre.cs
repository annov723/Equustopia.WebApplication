namespace Equustopia.WebApplication.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("equestrianCentre", Schema = "main")]
    public class EquestrianCentre
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        
        [Required]
        [MaxLength(250)]
        public string name { get; set; }
        
        [Required]
        public int userId { get; set; }
        
        public double? latitude { get; set; }
        public double? Longitude { get; set; }

        [MaxLength(250)]
        public string address { get; set; }
        
        public UserData UserData { get; set; }
        public ICollection<Horse> Horses { get; set; }
        
        
    }
}