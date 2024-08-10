using System.ComponentModel.DataAnnotations;

namespace EventSphere.Common.Attributes;

public class ValidEventTypesAttribute : ValidationAttribute
{
    private readonly List<string> _validEventTypes =
    [
        "General",
        "Sport",
        "Art",
        "Science",
        "Technology",
        "Gaming",
        "Adventure"
    ];
    
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is List<string> eventTypes)
        {
            if (eventTypes.Any(eventType => !_validEventTypes.Contains(eventType)))
            {
                return new ValidationResult("There is an invalid event type in the list");
            }
        }

        return ValidationResult.Success!;
    }
}