using System.Security.Claims;
using EventSphere.Application.Mappers;
using EventSphere.Application.Services.Interfaces;
using EventSphere.Domain.Dtos;
using EventSphere.Domain.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventSphere.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountsController(
    IAccountService accountService,
    ITokenBlacklistService tokenBlacklistService,
    IConfiguration configuration) : ControllerBase
{
    private readonly TimeSpan _tokenExpiration =
        TimeSpan.FromMinutes(configuration.GetValue<int>("Jwt:ExpiryInMinutes"));

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
            return Unauthorized("Email is already taken.");
        }

        var user = await accountService.Register(userRegistrationRequestDto);

        return Created("", user.ToUserRegistrationResponseDto());
    }

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

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        tokenBlacklistService.BlacklistToken(token, _tokenExpiration);
        return Ok();
    }

    [HttpGet("google-signin")]
    public IActionResult GoogleSignIn()
    {
        var props = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse", "Accounts") };
        return Challenge(props, "GoogleLogin");
    }

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
}