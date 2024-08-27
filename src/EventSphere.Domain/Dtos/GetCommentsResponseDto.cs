namespace EventSphere.Domain.Dtos;

public record GetCommentsResponseDto(
    List<CommentResponseDto> Comments);