namespace Equustopia.WebApplication.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Uzytkownik", Schema = "main")]
    public class Uzytkownik
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Nazwa { get; set; }
        
        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [StringLength(60)]
        public string Haslo { get; set; }
        
        public ICollection<Osrodek> Osrodki { get; set; }
        public ICollection<Kon> Konie { get; set; }
        public ICollection<WyswietleniaStron> WyswietleniaStron { get; set; }
    }
}