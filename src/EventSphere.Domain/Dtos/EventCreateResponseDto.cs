namespace EventSphere.Domain.Dtos;

public record EventCreateResponseDto(
    int Id,
    string Title,
    string Description,
    string Location,
    int OwnerId,
    DateOnly Date,
    long StartTime,
    long EndTime,
    string? BannerPictureUrl,
    int LikesCount,
    List<string>? EventTypes = null!
    );