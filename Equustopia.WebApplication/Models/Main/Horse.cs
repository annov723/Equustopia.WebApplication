namespace Equustopia.WebApplication.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("horse", Schema = "main")]
    public class Horse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        [MaxLength(50)]
        public string name { get; set; }

        [Required]
        public int userId { get; set; }

        public int? centreId { get; set; }
        
        public UserData UserData { get; set; }
        public EquestrianCentre EquestrianCentre { get; set; }
    }
}