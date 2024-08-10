namespace EventSphere.Domain.Dtos;

/// <summary>
/// Data transfer object for user authentication response.
/// </summary>
/// <param name="Token">
/// The JWT token used for communication with the other endpoints
/// </param>
/// <param name="IsAuthenticated">
/// A boolean value indicating whether the user is authenticated or not.
/// </param>
public record UserAuthenticationResponseDto(
    string Token,
    bool IsAuthenticated,
    int Id
);