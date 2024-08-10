using EventSphere.Application.Mappers;
using EventSphere.Application.Services.Interfaces;
using EventSphere.Domain.Dtos;
using EventSphere.Domain.Enums;
using EventSphere.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventSphere.Api.Controllers;

/// <summary>
/// Controller for events operations.
/// </summary>
/// <param name="eventService">
/// Service for events operations.
/// </param>
[ApiController]
[Route("[controller]")]
public class EventsController(IEventService eventService, IAccountService accountService, JwtHandler jwtHandler)
    : ControllerBase
{
    /// <summary>
    /// An endpoint to list events based on the filter
    /// </summary>
    [Authorize]
    [HttpPost("list")]
    public async Task<IActionResult> List()
    {
        return Ok();
    }

    /// <summary>
    /// An endpoint to get the event based on the id.
    /// </summary>
    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetEvent(int id)
    {
        var eventObj = await eventService.GetEventById(id);

        if (eventObj is null)
            return BadRequest("The id of the event is not found in the database");
        
        return Ok(eventObj.ToEventCreateResponseDto());
    }

    /// <summary>
    /// An endpoint to create new events.
    /// </summary>
    [Authorize(Roles = Role.EventOrganizer)]
    [HttpPost("create")]
    public async Task<IActionResult> Create(EventCreateRequestDto eventCreateRequestDto)
    {
        var userEmail = jwtHandler.GetUserEmail(Request.Headers.Authorization);
        if (userEmail is null)
        {
            return Unauthorized("Not able to find the email from the authentication token.");
        }

        var user = await accountService.GetUserByEmail(userEmail);
        if (user is null)
        {
            return Unauthorized("Not able to find the email associated in the authentication token.");
        }

        var eventEntity = await eventService.Create(eventCreateRequestDto, user.Id);

        return Created("", eventEntity.ToEventCreateResponseDto());
    }

    /// <summary>
    /// An endpoint to delete events.
    /// </summary>
    [Authorize(Roles = Role.EventOrganizer)]
    [HttpGet("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await eventService.DoesEventExist(id))
            return BadRequest("The id of the event is not found in the database");
        
        var isSuccessfulDelete = await eventService.Delete(id);
        if (isSuccessfulDelete) return Ok();

        return UnprocessableEntity("Not able to delete the event data");
    }

    /// <summary>
    /// An endpoint to update events' information.
    /// </summary>
    [Authorize(Roles = Role.EventOrganizer)]
    [HttpPut("update")]
    public async Task<IActionResult> Update(EventUpdateRequestDto eventUpdateRequestDto)
    {
        if (!await eventService.DoesEventExist(eventUpdateRequestDto.Id!.Value))
            return BadRequest("The id of the event is not found in the database");

        var isSuccessfulUpdate = await eventService.Update(eventUpdateRequestDto);
        if (isSuccessfulUpdate) return Ok();

        return UnprocessableEntity("Not able to update the event data");
    }
}