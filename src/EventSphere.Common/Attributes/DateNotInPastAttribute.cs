using System.ComponentModel.DataAnnotations;

namespace EventSphere.Common.Attributes;

/// <summary>
/// A custom attribute to validate that a date is not in the past
/// </summary>
public class DateNotInPastAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is DateOnly date && date < DateOnly.FromDateTime(DateTime.Today))
        {
            return new ValidationResult("The date cannot be in the past.");
        }

        return ValidationResult.Success!;
    }
}