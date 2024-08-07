namespace EventSphere.Domain.Dtos;

/// <summary>
/// Data transfer object for user authentication request.
/// </summary>
public record UserAuthenticationRequestDto(
    string Email,
    string Password
    );