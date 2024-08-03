using EventSphere.Domain;
using EventSphere.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EventSphere.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountsController : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto userRegistrationRequestDto)
    {

        return Created();
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