namespace Equustopia.WebApplication.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Uzytkownik", Schema = "main")]
    public class Uzytkownik
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string nazwa { get; set; }
        
        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string email { get; set; }
        
        [Required]
        [StringLength(60)]
        public string haslo { get; set; }
        
        public ICollection<Osrodek> Osrodki { get; set; }
        public ICollection<Kon> Konie { get; set; }
        public ICollection<WyswietleniaStron> WyswietleniaStron { get; set; }
    }
}