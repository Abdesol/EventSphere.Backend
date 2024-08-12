using EventSphere.Domain.Dtos;
using EventSphere.Domain.Entities;

namespace EventSphere.Application.Mappers;

/// <summary>
/// Event mapper class to map event entity to and from event dtos.
/// </summary>
public static class EventMapper
{
    public static Event ToEntity(this EventCreateRequestDto eventCreateRequestDto, int? id = null, int? ownerId = null)
    {
        var eventObj = new Event
        {
            Title = eventCreateRequestDto.Title,
            Description = eventCreateRequestDto.Description,
            Location = eventCreateRequestDto.Location,
            EventTypes = eventCreateRequestDto.EventTypes,
            StartTime = eventCreateRequestDto.StartTime,
            EndTime = eventCreateRequestDto.EndTime
        };
        
        if (id is not null)
            eventObj.Id = (int)id;
        
        if (ownerId is not null)
            eventObj.OwnerId = (int)ownerId;

        return eventObj;
    }

    public static EventCreateResponseDto ToEventCreateResponseDto(this Event eventEntity)
    {
        return new EventCreateResponseDto(eventEntity.Id, eventEntity.Title!, eventEntity.Description!,
            eventEntity.Location!, eventEntity.OwnerId, eventEntity.Date, eventEntity.StartTime, eventEntity.EndTime,
            eventEntity.EventTypes);
    }
    
    public static ListResponseDto ToEventListResponseDto(this List<Event> eventEntities)
    {
        return new ListResponseDto(eventEntities.ConvertAll(ToEventCreateResponseDto));
    }
}