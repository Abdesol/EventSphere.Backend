using EventSphere.Domain.Dtos;
using EventSphere.Domain.Entities;

namespace EventSphere.Application.Mappers;

public static class UserMapper
{
    public static UserRegistrationResponseDto ToUserRegistrationResponseDto(this User user)
    {
        return new UserRegistrationResponseDto(user.Id, user.Username!, user.Email!);
    }
}