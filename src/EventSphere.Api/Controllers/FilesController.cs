using EventSphere.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventSphere.Api.Controllers;

/// <summary>
/// Controller for file operations like retrieve and upload
/// </summary>
[ApiController]
[Route("[controller]")]
public class FilesController(IFileService fileService) : ControllerBase
{
    /// <summary>
    /// Download file endpoint based on the id of the file in the database.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> DownloadFile(string id)
    {
        try
        {
            var fileResult = await fileService.GetFileById(id);
            if (fileResult == null)
            {
                return NotFound();
            }

            return File(fileResult.FileStream, fileResult.ContentType, fileResult.FileName);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Upload file endpoint which responds with the id of the file in the database.
    /// </summary>
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        try
        {
            var fileUploadResponse = await fileService.SaveFile(file);
            return Ok(fileUploadResponse);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}