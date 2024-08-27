using System.ComponentModel.DataAnnotations;

namespace EventSphere.Domain.Dtos;

/// <summary>
/// Request DTO for creating a comment
/// </summary>
/// <param name="EventId">Event id that the comment is related to</param>
/// <param name="Content">Content of the comment</param>
public record CommentRequestDto(
    [Required] int? EventId,
    [Required] [MinLength(2)] [MaxLength(200)] string Content
    );