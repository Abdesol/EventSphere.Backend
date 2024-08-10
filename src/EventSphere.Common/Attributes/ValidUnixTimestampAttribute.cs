using System.ComponentModel.DataAnnotations;

namespace EventSphere.Common.Attributes;

/// <summary>
/// A validation attribute to check if a value is a valid Unix timestamp.
/// </summary>
public class ValidUnixTimestampAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is long timestamp)
        {
            try
            {
                var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
                return ValidationResult.Success!;
            }
            catch (ArgumentOutOfRangeException)
            {
                return new ValidationResult("Invalid Unix timestamp.");
            }
        }

        return new ValidationResult("The value is not a valid Unix timestamp.");
    }
}
