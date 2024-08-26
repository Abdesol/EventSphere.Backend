using System.ComponentModel.DataAnnotations;
using EventSphere.Application.Mappers;
using EventSphere.Application.Services.Interfaces;
using EventSphere.Common.Enums;
using EventSphere.Domain.Dtos;
using EventSphere.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace EventSphere.Api.Controllers;

/// <summary>
/// Controller for events operations.
/// </summary>
/// <param name="eventService">
/// Service for events operations.
/// </param>
[ApiController]
[Route("[controller]")]
public class EventsController(
    IEventService eventService,
    IFileService fileService,
    IAccountService accountService,
    JwtHandler jwtHandler)
    : ControllerBase
{
    private static readonly List<string> ValidEventTypes = EventTypes.All();

    /// <summary>
    /// List the event types that are acceptable
    /// </summary>
    /// <returns></returns>
    [HttpGet("event-types")]
    public IActionResult GetEventTypes()
    {
        return Ok(ValidEventTypes);
    }

    /// <summary>
    /// An endpoint to list events based on the filter. If no filter provided, gives all events in the range of today to 7 days ahead.
    /// </summary>
    [Authorize]
    [HttpPost("list")]
    public async Task<IActionResult> List(ListEventsRequestDto listEventsRequestDto)
    {
        var events = await eventService.GetEvents(listEventsRequestDto);


        var hostPath = $"{Request.Scheme}://{Request.Host}";
        return Ok(events.ToEventListResponseDto(hostPath));
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

        var hostPath = $"{Request.Scheme}://{Request.Host}";
        return Ok(eventObj.ToEventCreateResponseDto(hostPath));
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

        if (eventCreateRequestDto.BannerPictureId is not null)
        {
            var doesProfilePictureIdExist =
                await fileService.DoesFileExist(eventCreateRequestDto.BannerPictureId);
            if (!doesProfilePictureIdExist)
                return BadRequest("Banner picture id does not exist.");

            var isFileTypeOfImage = await fileService.IsFileTypeOfImage(eventCreateRequestDto.BannerPictureId);
            if (!isFileTypeOfImage)
                return BadRequest("Banner picture id is not an image type.");
        }

        var eventEntity = await eventService.Create(eventCreateRequestDto, user.Id);

        var hostPath = $"{Request.Scheme}://{Request.Host}";
        return Created("", eventEntity.ToEventCreateResponseDto(hostPath));
    }

    /// <summary>
    /// An endpoint to delete events.
    /// </summary>
    [Authorize(Roles = Role.EventOrganizer)]
    [HttpGet("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var (isVerified, output) = await VerifyUser(Request.Headers.Authorization, id);
        if (!isVerified)
            return output;

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
        var (isVerified, output) = await VerifyUser(Request.Headers.Authorization, eventUpdateRequestDto.Id);
        if (!isVerified)
            return output;

        if (!await eventService.DoesEventExist(eventUpdateRequestDto.Id))
            return BadRequest("The id of the event is not found in the database");

        if (eventUpdateRequestDto.BannerPictureId is not null)
        {
            var doesProfilePictureIdExist =
                await fileService.DoesFileExist(eventUpdateRequestDto.BannerPictureId);
            if (!doesProfilePictureIdExist)
                return BadRequest("Profile picture id does not exist.");

            var isFileTypeOfImage = await fileService.IsFileTypeOfImage(eventUpdateRequestDto.BannerPictureId);
            if (!isFileTypeOfImage)
                return BadRequest("Profile picture id is not an image type.");
        }

        var isSuccessfulUpdate = await eventService.Update(eventUpdateRequestDto);
        if (isSuccessfulUpdate) return Ok();

        return UnprocessableEntity("Not able to update the event data");
    }

    /// <summary>
    /// An endpoint to set a banner picture for an event.
    /// </summary>
    /// <param name="eventId">the event id to set the banner picture to</param>
    /// <param name="bannerId">id of the banner picture</param>
    [Authorize]
    [HttpGet("set-banner-picture")]
    public async Task<IActionResult> SetBannerPicture(
        [FromQuery] [Required] int? eventId,
        [FromQuery] [Required] string? bannerId)
    {
        var (isVerified, output) = await VerifyUser(Request.Headers.Authorization, eventId!.Value);
        if (!isVerified)
            return output;

        var doesBannerPictureIdExist =
            await fileService.DoesFileExist(bannerId!);

        if (!doesBannerPictureIdExist)
            return BadRequest("Banner picture id does not exist.");

        var isFileTypeOfImage = await fileService.IsFileTypeOfImage(bannerId!);
        if (!isFileTypeOfImage)
            return BadRequest("Banner picture id is not an image type.");

        var isSet = await eventService.SetBannerPicture(eventId.Value!, bannerId!);
        return isSet
            ? Ok("Banner picture updated successfully")
            : StatusCode(500, "An error occurred while setting the banner picture.");
    }

    /// <summary>
    /// A method to verify an event and a user
    /// </summary>
    /// <param name="authorization">the authorization header where we get user information from</param>
    /// <param name="eventId">the event id to verify against</param>
    private async Task<(bool isSuccess, ObjectResult output)> VerifyUser(StringValues authorization, int eventId)
    {
        var userEmail = jwtHandler.GetUserEmail(authorization);
        if (userEmail is null)
        {
            return (false, Unauthorized("Not able to find the email from the authentication token."));
        }

        var user = await accountService.GetUserByEmail(userEmail);
        if (user is null)
            return (false, Unauthorized("Not able to authenticate you with the authentication token."));

        var eventObj = await eventService.GetEventById(eventId);

        if (eventObj is null)
            return (false, BadRequest("Event id doesn't exist."));

        if (user.Id != eventObj.OwnerId)
            return (false, Unauthorized("You are not the owner of the event."));

        return (true, null!);
    }
}