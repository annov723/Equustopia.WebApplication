namespace Equustopia.WebApplication.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("NajczesciejWyswietlaneStrony", Schema = "analytics")]
    public class NajczesciejWyswietlaneStrony
    {
        public int StronaId { get; set; }
    
        [Column(TypeName = "varchar(50)")]
        public string StronaTyp { get; set; }
    
        public int IloscWyswietlen { get; set; }
    
        public int Pozycja { get; set; }
    }
}