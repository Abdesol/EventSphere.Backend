using EventSphere.Application.Mappers;
using EventSphere.Application.Services.Interfaces;
using EventSphere.Domain.Dtos;
using EventSphere.Domain.Entities;
using EventSphere.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventSphere.Application.Services;

public class EventService(ApplicationDbContext appDbContext) : IEventService
{
    /// <inheritdoc />
    public async Task<Event> Create(EventCreateRequestDto eventCreateRequestDto, int ownerId)
    {
        var eventObj = eventCreateRequestDto.ToEntity(ownerId: ownerId);

        eventObj.Date = DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(eventCreateRequestDto.StartTime).Date);

        var eventEntry = await appDbContext.Events.AddAsync(eventObj);
        await appDbContext.SaveChangesAsync();

        return eventEntry.Entity;
    }

    /// <inheritdoc />
    public async Task<bool> Delete(int id)
    {
        var eventObj = await appDbContext.Events.FindAsync(id);
        if (eventObj is null) return false;

        appDbContext.Events.Remove(eventObj);
        await appDbContext.SaveChangesAsync();

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
            eventObj.Date = DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(eventObj.StartTime).Date);
        }

        await appDbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DoesEventExist(int id)
    {
        var eventObj = await appDbContext.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        return eventObj is not null;
    }

    public async Task<Event?> GetEventById(int id)
    {
        var eventObj = await appDbContext.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        return eventObj;
    }
}