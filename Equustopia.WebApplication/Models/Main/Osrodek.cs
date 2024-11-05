namespace Equustopia.WebApplication.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Osrodek", Schema = "main")]
    public class Osrodek
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        
        [Required]
        [MaxLength(250)]
        public string nazwa { get; set; }
        
        [Required]
        public int uzytkownikId { get; set; }
        
        public double? szerokoscGeograficzna { get; set; }
        public double? wysokoscGeograficzna { get; set; }

        [MaxLength(250)]
        public string adres { get; set; }
        
        public Uzytkownik Uzytkownik { get; set; }
        public ICollection<Kon> Konie { get; set; }
        
        
    }
}