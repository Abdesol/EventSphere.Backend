using System.ComponentModel.DataAnnotations;

namespace EventSphere.Common.Attributes;

/// <summary>
/// A custom attribute to validate that a date is not in the past
/// </summary>
public class UnixTimeNotInPastOrFarFutureAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is long time)
        {
            if (time < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                return new ValidationResult("The time cannot be in the past.");
            
            if (time > new DateTimeOffset(DateTime.UtcNow.AddYears(1)).ToUnixTimeSeconds())
                return new ValidationResult("The time cannot be more than a year in the future.");
        }

        return ValidationResult.Success!;
    }
}