using System.ComponentModel.DataAnnotations;
using EventSphere.Common.Enums;

namespace EventSphere.Common.Attributes;

public class ValidEventTypesAttribute : ValidationAttribute
{
    private static readonly List<string> ValidEventTypes = EventTypes.All();

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is List<string> eventTypes)
        {
            if (eventTypes.Any(eventType => !ValidEventTypes.Contains(eventType)))
            {
                return new ValidationResult(
                    $"There is an invalid event type in the list." +
                    $"Valid event types are: {string.Join(", ", ValidEventTypes)}");
            }
        }

        return ValidationResult.Success!;
    }
}