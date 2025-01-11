namespace Equustopia.WebApplication.Models.Attributes
{
    using System.ComponentModel.DataAnnotations;

    public class DateNotInFutureAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                return true;
            }
            
            if (value is DateTime dateValue)
            {
                return dateValue <= DateTime.Today;
            }
            return false;
        }
    }
}