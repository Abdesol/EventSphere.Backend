using EventSphere.Domain.Dtos;
using EventSphere.Domain.Entities;

namespace EventSphere.Application.Services.Interfaces;

/// <summary>
/// The event service interface
/// </summary>
public interface IEventService
{
    /// <summary>
    /// Create an event method.
    /// </summary>
    /// <param name="eventCreateRequestDto">
    /// The event create request data transfer object.
    /// </param>
    /// <returns></returns>
    public Task<Event> Create(EventCreateRequestDto eventCreateRequestDto);
}