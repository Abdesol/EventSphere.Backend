using EventSphere.Common.Attributes;

namespace EventSphere.Domain.Dtos;

public record ListEventsRequestDto(
    [ValidEventTypes] List<string>? EventTypes,
    [DefaultDate(0)] DateOnly? StartDate,
    [DefaultDate(7)] DateOnly? EndDate
);