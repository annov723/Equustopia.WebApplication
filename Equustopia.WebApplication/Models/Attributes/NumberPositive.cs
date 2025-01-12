namespace Equustopia.WebApplication.Models.Attributes
{
    using System.ComponentModel.DataAnnotations;

    public class NumberPositive : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                return true;
            }
            if (value is double doubleValue && doubleValue > 0)
            {
                return true;
            }
            return false;
        }
    }
}