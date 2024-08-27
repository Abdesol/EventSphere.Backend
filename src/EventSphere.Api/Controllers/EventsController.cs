using System.ComponentModel.DataAnnotations;
using EventSphere.Application.Mappers;
using EventSphere.Application.Services.Interfaces;
using EventSphere.Common.Enums;
using EventSphere.Domain.Dtos;
using EventSphere.Domain.Entities;
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
    ICommentService commentService,
    IFileService fileService,
    IAccountService accountService,
    JwtHandler jwtHandler)
    : ControllerBase
{
    private static readonly List<string> ValidEventTypes = EventTypes.All();

    /// <summary>
    /// List the event types that are acceptable
    /// </summary>
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
        var (isVerified, output, user) = await VerifyUser(Request.Headers.Authorization);
        if (!isVerified)
            return output;

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

        var eventEntity = await eventService.Create(eventCreateRequestDto, user!.Id);

        var hostPath = $"{Request.Scheme}://{Request.Host}";
        return Created("", eventEntity.ToEventCreateResponseDto(hostPath));
    }

    /// <summary>
    /// An endpoint to delete events.
    /// </summary>
    [Authorize(Roles = Role.EventOrganizer)]
    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var (isVerified, output, user, eventObj) = await VerifyUserForEvent(Request.Headers.Authorization, id);
        if (!isVerified)
            return output;

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
        var (isVerified, output, user, eventObj) =
            await VerifyUserForEvent(Request.Headers.Authorization, eventUpdateRequestDto.Id);
        if (!isVerified)
            return output;

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
        var (isVerified, output, user, eventObj) =
            await VerifyUserForEvent(Request.Headers.Authorization, eventId!.Value);
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
    /// An endpoint to like an event.
    /// </summary>
    /// <param name="eventId">id of the event the user is liking</param>
    [Authorize]
    [HttpGet("like")]
    public async Task<IActionResult> Like([FromQuery] [Required] int? eventId)
    {
        var (isVerified, output, user, eventObj) =
            await VerifyUserForEvent(Request.Headers.Authorization, eventId!.Value, false);
        if (!isVerified)
            return output;

        var (isSuccess, errorMessage) = await eventService.LikeEvent(eventId.Value, user!.Id);

        if (!isSuccess) return BadRequest(errorMessage);

        return Ok();
    }

    /// <summary>
    /// An endpoint to unlike an event.
    /// </summary>
    /// <param name="eventId">id of the event the user is unliking</param>
    [Authorize]
    [HttpGet("unlike")]
    public async Task<IActionResult> Unlike([FromQuery] [Required] int? eventId)
    {
        var (isVerified, output, user, eventObj) =
            await VerifyUserForEvent(Request.Headers.Authorization, eventId!.Value, false);
        if (!isVerified)
            return output;

        var (isSuccess, errorMessage) = await eventService.UnlikeEvent(eventId.Value, user!.Id);

        if (!isSuccess) return BadRequest(errorMessage);

        return Ok();
    }

    /// <summary>
    /// An endpoint to get comments of an event.
    /// </summary>
    /// <param name="eventId">id of the event to get comments from</param>
    [Authorize]
    [HttpGet("get-comments")]
    public async Task<IActionResult> GetComments([FromQuery] [Required] int? eventId)
    {
        if (!await eventService.DoesEventExist(eventId!.Value))
            return BadRequest("Event id doesn't exist.");
        
        var comments = await commentService.GetComments(eventId!.Value);

        var hostPath = $"{Request.Scheme}://{Request.Host}";
        return Ok(comments.ToGetCommentsResponseDto(hostPath));
    }

    /// <summary>
    /// An endpoint to comment on an event.
    /// </summary>
    [Authorize]
    [HttpPost("comment")]
    public async Task<IActionResult> Comment(CommentRequestDto commentRequestDto)
    {
        var (isVerified, output, user) = await VerifyUser(Request.Headers.Authorization);
        if (!isVerified)
            return output;

        if (!await eventService.DoesEventExist(commentRequestDto.EventId!.Value))
            return BadRequest("Event id doesn't exist.");
        
        var comment = await commentService.Create(commentRequestDto, user!.Id);
        var hostPath = $"{Request.Scheme}://{Request.Host}";

        return Created("", comment.ToResponseDto(hostPath));
    }

    /// <summary>
    /// An endpoint to update a comment on an event.
    /// </summary>
    [Authorize]
    [HttpPut("update-comment")]
    public async Task<IActionResult> UpdateComment(CommentUpdateRequestDto commentUpdateRequestDto)
    {
        var (isVerified, output) =
            await VerifyUserForComment(Request.Headers.Authorization, commentUpdateRequestDto.Id!.Value);
        if (!isVerified)
            return output;

        if (!await eventService.DoesEventExist(commentUpdateRequestDto.EventId!.Value))
            return BadRequest("Event id doesn't exist.");

        var isSuccessfulUpdate = await commentService.Update(commentUpdateRequestDto);
        if (isSuccessfulUpdate) return Ok();

        return UnprocessableEntity("Not able to update the comment data");
    }

    /// <summary>
    /// An endpoint to delete a comment on an event.
    /// </summary>
    [Authorize]
    [HttpDelete("delete-comment")]
    public async Task<IActionResult> DeleteComment([FromQuery] [Required] int? id, [FromQuery] [Required] int? eventId)
    {
        var (isVerified, output) = await VerifyUserForComment(Request.Headers.Authorization, id!.Value);
        if (!isVerified)
            return output;
        
        if (!await eventService.DoesEventExist(eventId!.Value))
            return BadRequest("Event id doesn't exist.");
        
        var isSuccessfulDelete = await commentService.Delete(id.Value, eventId!.Value);
        if (isSuccessfulDelete) return Ok();

        return UnprocessableEntity("Not able to delete the comment data");
    }

    /// <summary>
    /// A method to verify a user
    /// </summary>
    /// <param name="authorization">the authorization header where we get user information from</param>
    private async Task<(bool isSuccess, ObjectResult output, User? user)> VerifyUser(StringValues authorization)
    {
        var userEmail = jwtHandler.GetUserEmail(authorization);
        if (userEmail is null)
        {
            return (false, Unauthorized("Not able to find the email from the authentication token."), null);
        }

        var user = await accountService.GetUserByEmail(userEmail);
        if (user is null)
            return (false, Unauthorized("Not able to authenticate you with the authentication token."), null);

        return (true, null!, user);
    }

    /// <summary>
    /// A method to verify an event and a user
    /// </summary>
    /// <param name="authorization">the authorization header where we get user information from</param>
    /// <param name="eventId">the event id to verify against</param>
    /// <param name="shouldBeOwner">indicates if it has to check for event id owner with user id match or not</param>
    private async Task<(bool isSuccess, ObjectResult output, User? user, Event? eventObj)> VerifyUserForEvent(
        StringValues authorization, int eventId, bool shouldBeOwner = true)
    {
        var (isVerified, output, user) = await VerifyUser(authorization);

        if (!isVerified)
            return (false, output, null, null);

        var eventObj = await eventService.GetEventById(eventId);

        if (eventObj is null)
            return (false, BadRequest("Event id doesn't exist."), user, null);

        if (shouldBeOwner && user.Id != eventObj.OwnerId)
            return (false, Unauthorized("You are not the owner of the event."), user, eventObj);

        return (true, null!, user, eventObj);
    }

    /// <summary>
    /// A method to verify a comment and user
    /// </summary>
    /// <param name="authorization">the authorization header where we get user information from</param>
    /// <param name="commentId">the comment id to verify against</param>
    private async Task<(bool isSuccess, ObjectResult output)> VerifyUserForComment(StringValues authorization,
        int commentId)
    {
        var (isVerified, output, user) = await VerifyUser(authorization);

        if (!isVerified)
            return (false, output);

        var comment = await commentService.GetCommentById(commentId);

        if (comment is null)
            return (false, BadRequest("Comment id doesn't exist."));

        if (user.Id != comment.UserId)
            return (false, Unauthorized("You are not the owner of the comment."));

        return (true, null!);
    }
}