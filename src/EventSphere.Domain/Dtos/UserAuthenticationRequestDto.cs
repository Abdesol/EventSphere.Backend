namespace EventSphere.Domain.Dtos;

public record UserAuthenticationRequestDto(
    string Email,
    string Password
    );