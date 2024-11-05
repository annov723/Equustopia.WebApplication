namespace Equustopia.WebApplication.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Kon", Schema = "main")]
    public class Kon
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        [MaxLength(50)]
        public string nazwa { get; set; }

        [Required]
        public int uzytkownikId { get; set; }

        public int? osrodekId { get; set; }
        
        public Uzytkownik Uzytkownik { get; set; }
        public Osrodek Osrodek { get; set; }
    }
}