namespace Equustopia.WebApplication.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Osrodek", Schema = "main")]
    public class Osrodek
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(250)]
        public string Nazwa { get; set; }
        
        [Required]
        public int UzytkownikId { get; set; }
        
        public double? SzerokoscGeograficzna { get; set; }
        public double? WysokoscGeograficzna { get; set; }

        [MaxLength(250)]
        public string Adres { get; set; }
        
        public Uzytkownik Uzytkownik { get; set; }
        public ICollection<Kon> Konie { get; set; }
        
        
    }
}