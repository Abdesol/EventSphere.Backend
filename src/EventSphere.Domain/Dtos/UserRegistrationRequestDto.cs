using System.ComponentModel.DataAnnotations;

namespace EventSphere.Domain.Dtos;

public record UserRegistrationRequestDto(
    [Required] [MaxLength(50)] string Email,
    [Required] [MinLength(3)] [MaxLength(10)] string Username,
    [Required] [MinLength(8)]  [MaxLength(32)] string Password,
    bool IsEventOrganizer = false);