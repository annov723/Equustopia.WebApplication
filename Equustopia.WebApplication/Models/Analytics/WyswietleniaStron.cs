namespace Equustopia.WebApplication.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("WyswietleniaStron", Schema = "analytics")]
    public class WyswietleniaStron
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int? uzytkownikId { get; set; }

        [Required]
        public int stronaId { get; set; }

        [Required]
        [MaxLength(50)]
        public string stronaTyp { get; set; }

        public DateTime znacznikCzasu { get; set; } = DateTime.Now;

        [MaxLength(45)]
        public string adresIp { get; set; }
        
        public Uzytkownik Uzytkownik { get; set; }
    }
}