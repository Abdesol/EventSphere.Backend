using EventSphere.Application.Mappers;
using EventSphere.Application.Services.Interfaces;
using EventSphere.Domain;
using EventSphere.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EventSphere.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountsController(IAccountService accountService) : ControllerBase
{
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
    
    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] UserAuthenticationRequestDto userAuthenticationRequestDto)
    {

        return Created();
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] UserLogoutRequestDto userLogoutRequestDto)
    {

        return Created();
    }
}