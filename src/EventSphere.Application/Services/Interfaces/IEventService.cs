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
    /// <param name="ownerId">
    /// The owner event organizer user id
    /// </param>
    /// <returns></returns>
    public Task<Event> Create(EventCreateRequestDto eventCreateRequestDto, int ownerId);

    /// <summary>
    /// Delete the event from the database
    /// </summary>
    /// <param name="id">The id of the event to delete</param>
    /// <returns>true if successful otherwise false</returns>
    public Task<bool> Delete(int id);

    /// <summary>
    /// Updates the event based on the provided id
    /// </summary>
    /// <param name="eventUpdateRequestDto">the update request dto with updated data, but constant id</param>
    /// <returns>true if successfully updated otherwise false</returns>
    public Task<bool> Update(EventUpdateRequestDto eventUpdateRequestDto);

    /// <summary>
    /// Checks if the event exists or not
    /// </summary>
    /// <param name="id">id of the event to find in the database</param>
    /// <returns>true if it exists, otherwise false</returns>
    public Task<bool> DoesEventExist(int id);

    /// <summary>
    /// Returns an event entity based on the id
    /// </summary>
    /// <param name="id">Id of the event</param>
    /// <returns>The event object if it exists, otherwise null</returns>
    public Task<Event?> GetEventById(int id);

    /// <summary>
    /// Gets events based on the filter from the listRequestDto
    /// </summary>
    /// <param name="listEventsRequestDto">The filters request dto</param>
    /// <returns>A list of events based on the filters</returns>
    public Task<List<Event>> GetEvents(ListEventsRequestDto listEventsRequestDto);

    /// <summary>
    /// Sets the banner picture for the event
    /// </summary>
    /// <param name="eventId">id of the event</param>
    /// <param name="id">the banner picture id in the database</param>
    /// <returns>true if successfully set</returns>
    public Task<bool> SetBannerPicture(int eventId, string id);

    /// <summary>
    /// Likes the event
    /// </summary>
    /// <param name="eventId">the id of the event to like</param>
    /// <param name="userId">the id of the user liking the event</param>
    /// <returns>if liked successfully, it returns true and empty string, otherwise, if it is false, it returns false and the error in the string</returns>
    public Task<(bool, string?)> LikeEvent(int eventId, int userId);
    
    /// <summary>
    /// Unlikes the event
    /// </summary>
    /// <param name="eventId">the id of the event to unlike</param>
    /// <param name="userId">the id of the user unliking the event</param>
    /// <returns>if unliked successfully, it returns true and empty string, otherwise, if it is false, it returns false and the error in the string</returns>
    public Task<(bool, string?)> UnlikeEvent(int eventId, int userId);
}