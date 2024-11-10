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
        [MaxLength(50)]
        public required string name { get; set; }

        [Required]
        public required int userId { get; set; }

        public int? centreId { get; set; }
        
        public required UserData UserData { get; set; }
        public EquestrianCentre EquestrianCentre { get; set; }
    }
}