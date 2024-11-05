namespace Equustopia.WebApplication.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("WyswietleniaStron", Schema = "analytics")]
    public class WyswietleniaStron
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? UzytkownikId { get; set; }

        [Required]
        public int StronaId { get; set; }

        [Required]
        [MaxLength(50)]
        public string StronaTyp { get; set; }

        public DateTime ZnacznikCzasu { get; set; } = DateTime.Now;

        [MaxLength(45)]
        public string AdresIp { get; set; }
        
        public Uzytkownik Uzytkownik { get; set; }
    }
}