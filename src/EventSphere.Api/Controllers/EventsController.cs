using EventSphere.Application.Mappers;
using EventSphere.Application.Services.Interfaces;
using EventSphere.Domain.Dtos;
using EventSphere.Domain.Enums;
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
public class EventsController(IEventService eventService, IAccountService accountService) : ControllerBase
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
    /// An endpoint to create new events.
    /// </summary>
    [Authorize(Roles = Role.EventOrganizer)]
    [HttpPost("create")]
    public async Task<IActionResult> Create(EventCreateRequestDto eventCreateRequestDto)
    {
        if (!await accountService.DoesUserExist(eventCreateRequestDto.OwnerId))
        {
            return BadRequest("OwnerId does not exist.");
        }

        var eventEntity = await eventService.Create(eventCreateRequestDto);

        return Created("", eventEntity.ToEventCreateResponseDto());
    }

    /// <summary>
    /// An endpoint to delete events.
    /// </summary>
    [Authorize(Roles = Role.EventOrganizer)]
    [HttpGet("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        return Ok();
    }

    /// <summary>
    /// An endpoint to update events' information.
    /// </summary>
    [Authorize(Roles = Role.EventOrganizer)]
    [HttpPut("update/{id:int}")]
    public async Task<IActionResult> Update(int id)
    {
        return Ok();
    }
}