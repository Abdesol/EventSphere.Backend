using EventSphere.Domain.Dtos;
using EventSphere.Domain.Entities;

namespace EventSphere.Application.Mappers;

/// <summary>
/// Event mapper class to map event entity to and from event dtos.
/// </summary>
public static class EventMapper
{
    public static EventCreateResponseDto ToEventCreateResponseDto(this Event eventEntity)
    {
        return new EventCreateResponseDto(eventEntity.Id, eventEntity.Title!, eventEntity.Description!,
            eventEntity.Location!, eventEntity.OwnerId, eventEntity.Date, eventEntity.StartTime, eventEntity.EndTime,
            eventEntity.EventTypes);
    }
}