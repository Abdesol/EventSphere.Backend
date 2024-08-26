using EventSphere.Application.Mappers;
using EventSphere.Application.Services.Interfaces;
using EventSphere.Domain.Dtos;
using EventSphere.Domain.Entities;
using EventSphere.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EventSphere.Application.Services;

public class EventService(IMemoryCache cache, IFileService fileService, ApplicationDbContext appDbContext) : IEventService
{
    private const string EventCachePrefix = "Event_";
    private static readonly TimeSpan EventCacheExpirationInMinutes = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan EventCacheSlidingExpirationInMinutes = TimeSpan.FromMinutes(1);

    /// <inheritdoc />
    public async Task<Event> Create(EventCreateRequestDto eventCreateRequestDto, int ownerId)
    {
        var eventObj = eventCreateRequestDto.ToEntity(ownerId: ownerId);

        var dateTime = DateTimeOffset.FromUnixTimeSeconds(eventCreateRequestDto.StartTime).UtcDateTime;
        eventObj.Date = new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);

        var eventEntry = await appDbContext.Events.AddAsync(eventObj);
        await appDbContext.SaveChangesAsync();

        if (eventCreateRequestDto.BannerPictureId is not null)
        {
            fileService.FileIsUsed(eventCreateRequestDto.BannerPictureId);
        }

        return eventEntry.Entity;
    }

    /// <inheritdoc />
    public async Task<bool> Delete(int id)
    {
        var eventObj = await appDbContext.Events.FindAsync(id);
        if (eventObj is null) return false;

        appDbContext.Events.Remove(eventObj);
        await appDbContext.SaveChangesAsync();
        
        var cacheKey = EventCachePrefix + id;
        cache.Remove(cacheKey);

        return true;
    }

    /// <inheritdoc />
    public async Task<bool> Update(EventUpdateRequestDto eventUpdateRequestDto)
    {
        var eventObj = await appDbContext.Events.FindAsync(eventUpdateRequestDto.Id);
        if (eventObj is null) return false;

        eventObj.Title = eventUpdateRequestDto.Title ?? eventObj.Title;
        eventObj.Description = eventUpdateRequestDto.Description ?? eventObj.Description;
        eventObj.Location = eventUpdateRequestDto.Location ?? eventObj.Location;
        eventObj.EventTypes = eventUpdateRequestDto.EventTypes ?? eventObj.EventTypes;
        eventObj.StartTime = eventUpdateRequestDto.StartTime ?? eventObj.StartTime;
        eventObj.EndTime = eventUpdateRequestDto.EndTime ?? eventObj.EndTime;

        if (eventUpdateRequestDto.StartTime is not null)
        {
            var dateTime = DateTimeOffset.FromUnixTimeSeconds(eventObj.StartTime).UtcDateTime;
            eventObj.Date = new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
        }

        await appDbContext.SaveChangesAsync();
        
        var cacheKey = EventCachePrefix + eventUpdateRequestDto.Id;
        cache.Remove(cacheKey);

        if (eventUpdateRequestDto.BannerPictureId is not null)
        {
            fileService.FileIsUsed(eventUpdateRequestDto.BannerPictureId);
        }

        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DoesEventExist(int id)
    {
        var eventObj = await appDbContext.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        return eventObj is not null;
    }

    /// <inheritdoc />
    public async Task<Event?> GetEventById(int id)
    {
        var cacheKey = EventCachePrefix + id;
        
        if (cache.TryGetValue(cacheKey, out Event? cachedEvent))
        {
            return cachedEvent!;
        }
        
        var eventObj = await appDbContext.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        
        if (eventObj is not null)
        {
            cache.Set(cacheKey, eventObj, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = EventCacheExpirationInMinutes,
                SlidingExpiration = EventCacheSlidingExpirationInMinutes
            });
        }
        
        return eventObj;
    }

    /// <inheritdoc />
    public async Task<List<Event>> GetEvents(ListEventsRequestDto listEventsRequestDto)
    {
        var query = appDbContext.Events.AsNoTracking().AsQueryable();
        if (listEventsRequestDto.EventTypes is not null)
        {
            query = query.Where(
                e =>
                    e.EventTypes != null &&
                    e.EventTypes.Any(et => listEventsRequestDto.EventTypes.Contains(et)));
        }

        query = query.Where(e => e.Date >= listEventsRequestDto.StartDate && e.Date <= listEventsRequestDto.EndDate);

        return await query.ToListAsync();
    }

    public async Task<bool> SetBannerPicture(int eventId, string id)
    {
        var eventObj = await appDbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);
        if (eventObj is null) return false;
        
        eventObj.BannerPictureId = id;
        appDbContext.Events.Update(eventObj);
        await appDbContext.SaveChangesAsync();

        fileService.FileIsUsed(id);
        
        var cacheKey = EventCachePrefix + eventId;
        cache.Remove(cacheKey);
        
        return true;
    }
}