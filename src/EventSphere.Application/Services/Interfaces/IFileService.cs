using EventSphere.Domain.Dtos;
using Microsoft.AspNetCore.Http;

namespace EventSphere.Application.Services.Interfaces;

/// <summary>
/// The file service interface
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Saves the file to the database
    /// </summary>
    /// <param name="file">the file to be saved</param>
    /// <returns>Id of the file in the database</returns>
    public Task<UploadFileResponseDto> SaveFile(IFormFile? file);
    
    /// <summary>
    /// Retrieves the file based on the id of it.
    /// </summary>
    /// <param name="id">id of the file</param>
    /// <returns>the download file response dto</returns>
    public Task<DownloadFileResponseDto?> GetFileById(string id);

    /// <summary>
    /// Remove file from the pending cache as it is used now
    /// </summary>
    /// <param name="id">id of the file to remove from pending</param>
    public void FileIsUsed(string id);
    
    /// <summary>
    /// Checks if the file id exists in the database
    /// </summary>
    /// <param name="id">id of the file in the database</param>
    /// <returns>true if it exists, otherwise, false</returns>
    public Task<bool> DoesFileExist(string id);
    
    /// <summary>
    /// Checks if the file is type of image
    /// </summary>
    /// <param name="id">id of the file in the database</param>
    /// <returns>true if it is an image, otherwise, false</returns>
    public Task<bool> IsFileTypeOfImage(string id);
}