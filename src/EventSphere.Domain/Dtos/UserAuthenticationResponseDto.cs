namespace EventSphere.Domain.Dtos;

public record UserAuthenticationResponseDto(
    string Token,
    bool IsAuthenticated
);