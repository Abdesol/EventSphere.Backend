namespace EventSphere.Domain.Dtos;

public record ListEventsResponseDto(
    List<EventCreateResponseDto> Events
);