using System.ComponentModel.DataAnnotations;

namespace EventSphere.Common.Attributes;

public class DefaultDateAttribute(int daysFromNow) : ValidationAttribute
{
    private readonly DateOnly _defaultDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(daysFromNow));

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || (value is DateOnly dateOnly && dateOnly == default(DateOnly)))
        {
            var property = validationContext.ObjectType.GetProperty(validationContext.MemberName);
            if (property != null && property.CanWrite)
            {
                property.SetValue(validationContext.ObjectInstance, _defaultDate);
            }
        }

        return ValidationResult.Success;
    }
}