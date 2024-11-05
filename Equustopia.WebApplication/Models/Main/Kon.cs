namespace Equustopia.WebApplication.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Kon", Schema = "main")]
    public class Kon
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nazwa { get; set; }

        [Required]
        public int UzytkownikId { get; set; }

        public int? OsrodekId { get; set; }
        
        public Uzytkownik Uzytkownik { get; set; }
        public Osrodek Osrodek { get; set; }
    }
}