using System.Security.Claims;
using EventSphere.Application.Mappers;
using EventSphere.Application.Services.Interfaces;
using EventSphere.Common.Enums;
using EventSphere.Domain.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventSphere.Api.Controllers;

/// <summary>
/// Controller for account operations.
/// </summary>
/// <param name="accountService">
/// Service for account operations.
/// </param>
/// <param name="tokenBlacklistService">
/// Service for token blacklist operations.
/// </param>
/// <param name="configuration">
/// Configuration for the application.
/// </param>
[ApiController]
[Route("[controller]")]
public class AccountsController(
    IAccountService accountService,
    ITokenBlacklistService tokenBlacklistService,
    IConfiguration configuration) : ControllerBase
{
    private readonly TimeSpan _tokenExpiration =
        TimeSpan.FromMinutes(configuration.GetValue<int>("JwtSettings:ExpiryInMinutes"));

    /// <summary>
    /// An endpoint to get user details by id
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await accountService.GetUserById(id);

        if (user is null)
            return BadRequest("The id of the user is not found in the database");

        return Ok(user.ToUserRegistrationResponseDto());
    }
    
    /// <summary>
    /// Endpoint to register a new user.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto userRegistrationRequestDto)
    {
        var isThereSimilarUsername = await accountService.IsThereSimilarUsernames(userRegistrationRequestDto.Username);
        if (isThereSimilarUsername)
        {
            return BadRequest("Username is already taken.");
        }

        var isThereSimilarEmail = await accountService.IsThereSimilarEmails(userRegistrationRequestDto.Email);
        if (isThereSimilarEmail)
        {
            return BadRequest("Email is already taken.");
        }

        var user = await accountService.Register(userRegistrationRequestDto);

        return Created("", user.ToUserRegistrationResponseDto());
    }

    /// <summary>
    /// Endpoint to authenticate a user.
    /// </summary>
    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] UserAuthenticationRequestDto userAuthenticationRequestDto)
    {
        if (await accountService.IsUserRegisteredWithOAuth(userAuthenticationRequestDto.Email))
        {
            return Conflict();
        }

        var response = await accountService.Authenticate(userAuthenticationRequestDto);

        if (response == null)
        {
            return Unauthorized();
        }

        return Ok(response);
    }

    /// <summary>
    /// An endpoint protected with jwt authorization to logout a user.
    /// </summary>
    [Authorize]
    [HttpGet("logout")]
    public IActionResult Logout()
    {
        var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        tokenBlacklistService.BlacklistToken(token, _tokenExpiration);
        return Ok();
    }

    /// <summary>
    /// An endpoint to sign in with google.
    /// </summary>
    [HttpGet("google-signin")]
    public IActionResult GoogleSignIn()
    {
        var props = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse", "Accounts") };
        return Challenge(props, "GoogleLogin");
    }

    /// <summary>
    /// An endpoint to handle google response.
    /// </summary>
    [HttpGet("google-response")]
    public async Task<IActionResult> GoogleResponse()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!result.Succeeded)
        {
            return BadRequest("Google authentication failed.");
        }

        var email = result.Principal.FindFirstValue(ClaimTypes.Email);

        if (email is null)
        {
            return BadRequest("Google authentication failed.");
        }

        if (!await accountService.IsThereSimilarEmails(email))
        {
            var generatedUsername = email.Split("@")[0] + "_" + Guid.NewGuid().ToString()[..5];
            await accountService.Register(
                new UserRegistrationRequestDto(email, generatedUsername, ""), true, OAuthClient.Google);
        }

        // extra caution
        var user = await accountService.GetUserByEmail(email);

        if (user is null)
        {
            return Unauthorized();
        }

        var response = await accountService.Authenticate(new UserAuthenticationRequestDto(user.Email!, ""), true);

        if (response == null)
        {
            return Unauthorized();
        }

        return Ok(response);
    }
    
    /// <summary>
    /// An endpoint to promote a user to event organizer.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize]
    [HttpGet("promote-to-event-organizer/{id:int}")]
    public async Task<IActionResult> PromoteToEventOrganizer(int id)
    {
        if (!await accountService.DoesUserExist(id))
        {
            return BadRequest("User id does not exist.");
        }
        
        if (await accountService.IsUserAlreadyAnEventOrganizer(id))
        {
            return BadRequest("User is already an event organizer");
        }

        var updateRoleSuccessful = await accountService.PromoteToEventOrganizer(id);
        if (!updateRoleSuccessful)
        {
            return UnprocessableEntity("Could not promote user to event organizer.");
        }

        var newToken = await accountService.GenerateTokenByUserId(id);
        if (newToken is null)
        {
            return UnprocessableEntity("Could not create a new token");
        }
        
        return Ok(new PromoteToEventOrganizerResponseDto(newToken));
    } 
}