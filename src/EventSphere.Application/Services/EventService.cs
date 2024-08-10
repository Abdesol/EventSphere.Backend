using EventSphere.Application.Services.Interfaces;
using EventSphere.Domain.Dtos;
using EventSphere.Domain.Entities;
using EventSphere.Infrastructure.Data;

namespace EventSphere.Application.Services;

public class EventService(ApplicationDbContext appDbContext) : IEventService
{
    /// <inheritdoc />
    public async Task<Event> Create(EventCreateRequestDto eventCreateRequestDto)
    {
        return new Event();
    }
}