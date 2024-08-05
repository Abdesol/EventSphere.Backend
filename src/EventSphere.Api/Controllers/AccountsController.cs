using EventSphere.Application.Mappers;
using EventSphere.Application.Services.Interfaces;
using EventSphere.Domain;
using EventSphere.Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventSphere.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountsController(IAccountService accountService, ITokenBlacklistService tokenBlacklistService, IConfiguration configuration) : ControllerBase
{
    private readonly TimeSpan _tokenExpiration = TimeSpan.FromMinutes(configuration.GetValue<int>("Jwt:ExpiryInMinutes"));
    
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
}