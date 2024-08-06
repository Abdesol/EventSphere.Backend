using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security;
using EventSphere.Common.Attributes;

namespace EventSphere.Domain.Dtos;

public record UserRegistrationRequestDto(
    [Required] [MaxLength(50)] [EmailAddress] string Email,
    [Required] [MinLength(3)] [MaxLength(10)] [Alphanumeric] string Username,
    [Required] [MaxLength(32)] [PasswordPropertyText] string Password,
    bool IsEventOrganizer = false);