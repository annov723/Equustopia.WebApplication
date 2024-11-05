namespace Equustopia.WebApplication.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("NajczesciejWyswietlaneStrony", Schema = "analytics")]
    public class NajczesciejWyswietlaneStrony
    {
        public int stronaId { get; set; }
    
        [Column(TypeName = "varchar(50)")]
        public string stronaTyp { get; set; }
    
        public int iloscWyswietlen { get; set; }
    
        public int pozycja { get; set; }
    }
}