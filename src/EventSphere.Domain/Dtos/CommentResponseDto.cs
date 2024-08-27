namespace EventSphere.Domain.Dtos;

/// <summary>
/// Response DTO for the comment
/// </summary>
/// <param name="Id">Id of the comment</param>
/// <param name="Username">Username of the user who commented</param>
/// <param name="ProfilePictureUrl">Profile picture url of the user who commented</param>
/// <param name="Content">Content of the comment</param>
/// <param name="CreatedAt">The time the comment was created</param>
/// <param name="UpdatedAt">The time the comment was updated</param>
public record CommentResponseDto(
    int Id,
    string Username,
    string? ProfilePictureUrl,
    string Content,
    long CreatedAt,
    long UpdatedAt
);