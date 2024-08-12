namespace EventSphere.Domain.Dtos;

public record ListResponseDto(
    List<EventCreateResponseDto> Events
);