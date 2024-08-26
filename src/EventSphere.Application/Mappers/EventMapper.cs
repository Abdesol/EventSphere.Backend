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
            BannerPictureId = eventCreateRequestDto.BannerPictureId,
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

    public static EventCreateResponseDto ToEventCreateResponseDto(this Event eventEntity, string hostPath)
    {
        string? bannerPictureUrl = null;
        if (eventEntity.BannerPictureId != null)
        {
            bannerPictureUrl = $"{hostPath}/files/{eventEntity.BannerPictureId}";
        }

        return new EventCreateResponseDto(eventEntity.Id, eventEntity.Title!, eventEntity.Description!,
            eventEntity.Location!, eventEntity.OwnerId, eventEntity.Date, eventEntity.StartTime, eventEntity.EndTime,
            bannerPictureUrl, eventEntity.EventTypes);
    }

    public static ListEventsResponseDto ToEventListResponseDto(this List<Event> eventEntities, string hostPath)
    {
        return new ListEventsResponseDto(
            eventEntities.ConvertAll(eventEntity =>
                eventEntity.ToEventCreateResponseDto(hostPath)));
    }
}