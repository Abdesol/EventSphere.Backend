using EventSphere.Domain.Dtos;
using EventSphere.Domain.Entities;

namespace EventSphere.Application.Mappers;

/// <summary>
/// User mapper class to map user entity to and from user dtos.
/// </summary>
public static class UserMapper
{
    /// <summary>
    /// Maps user registration dto to user entity.
    /// </summary>
    public static UserRegistrationResponseDto ToUserRegistrationResponseDto(this User user)
    {
        return new UserRegistrationResponseDto(user.Id, user.Username!, user.Email!);
    }
}