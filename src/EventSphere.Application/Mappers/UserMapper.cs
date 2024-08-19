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
    public static UserRegistrationResponseDto ToUserRegistrationResponseDto(this User user, string hostPath)
    {
        string? profilePicturePath = null;
        if (user.ProfilePictureId != null)
        {
            profilePicturePath = $"{hostPath}/files/{user.ProfilePictureId}";
        }
        return new UserRegistrationResponseDto(user.Id, user.Username!, user.Email!, profilePicturePath);
    }
}