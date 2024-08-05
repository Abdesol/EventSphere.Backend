using System.ComponentModel.DataAnnotations;

namespace EventSphere.Domain.Dtos;

public record UserRegistrationResponseDto(
    int Id,
    [Required] string Email,
    [Required] string Username
    );