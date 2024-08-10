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
    List<string>? EventTypes = null!
    );