using Microsoft.AspNetCore.Mvc;

namespace EventSphere.Api.Controllers;

/// <summary>
/// The controller for the root path
/// </summary>
[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
    /// <summary>
    /// Get request on the root path for server checking
    /// </summary>
    [HttpGet("/")]
    public IActionResult Get()
    {
        return Ok("Hello, World!");
    }
}