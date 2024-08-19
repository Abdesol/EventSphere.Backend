using System.ComponentModel.DataAnnotations;

namespace EventSphere.Domain.Dtos;

/// <summary>
/// Data transfer object for user registration response.
/// </summary>
/// <param name="Id">
/// The id of the user generated after registration
/// </param>
/// <param name="Email">
/// The email provided for registration by the user, and it is required.
/// </param>
/// <param name="Username">
/// The username provided for registration by the user, and it is required.
/// </param>
public record UserRegistrationResponseDto(
    int Id,
    [Required] string Email,
    [Required] string Username,
    string? ProfilePicturePath
);