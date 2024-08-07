using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security;
using EventSphere.Common.Attributes;

namespace EventSphere.Domain.Dtos;

/// <summary>
/// Data transfer object for user registration request.
/// </summary>
/// <param name="Email">
/// Email address of the user. Should be unique, maximum length of 50 characters, and it is required.
/// </param>
/// <param name="Username">
/// Username of the user. Should be unique, character length in the range of 3 ~ 10, follow the alphanumeric format, and it is required.
/// </param>
/// <param name="Password">
/// Password of the user. Should have a maximum length of 32 characters, and it is required.
/// </param>
/// <param name="IsEventOrganizer">
/// Flag to determine if the user is an event organizer. Default value is false
/// </param>
public record UserRegistrationRequestDto(
    [Required] [MaxLength(50)] [EmailAddress] string Email,
    [Required] [MinLength(3)] [MaxLength(10)] [Alphanumeric] string Username,
    [Required] [MaxLength(32)] [PasswordPropertyText] string Password,
    bool IsEventOrganizer = false);