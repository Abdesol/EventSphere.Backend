using EventSphere.Domain.Dtos;
using EventSphere.Domain.Entities;

namespace EventSphere.Application.Mappers;

public static class CommentMapper
{
    public static CommentResponseDto ToResponseDto(this Comment comment, string hostPath)
    {
        string? profilePictureUrl = null;
        if (comment.User!.ProfilePictureId != null)
        {
            profilePictureUrl = $"{hostPath}/files/{comment.User!.ProfilePictureId}";
        }

        return new CommentResponseDto(
            comment.Id,
            comment.User.Username!,
            profilePictureUrl,
            comment.Content,
            comment.CreatedAt,
            comment.UpdatedAt);
    }

    public static GetCommentsResponseDto ToGetCommentsResponseDto(this List<Comment> comments, string hostPath)
    {
        return new GetCommentsResponseDto(
            comments.ConvertAll(comment =>
                comment.ToResponseDto(hostPath)));
    }
}