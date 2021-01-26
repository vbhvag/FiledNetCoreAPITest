using System;
using System.ComponentModel.DataAnnotations;

namespace Filed.PaymentProcess.API.Filters
{
    public class CheckDateRangeAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime dt = new DateTime();
            if (!DateTime.TryParse(Convert.ToString(value), out dt))
            {
                return new ValidationResult("Invalid Date");
            }

            if (dt > DateTime.UtcNow)
                return ValidationResult.Success;

            return new ValidationResult("Date must be greater than today");
        }
    }
}