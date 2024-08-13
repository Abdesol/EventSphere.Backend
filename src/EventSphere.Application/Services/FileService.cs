using EventSphere.Application.Services.Interfaces;
using EventSphere.Domain.Dtos;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;

namespace EventSphere.Application.Services;

public class FileService(IGridFSBucket gridFsBucket) : IFileService
{
    /// <inheritdoc />
    public async Task<UploadFileResponseDto> SaveFile(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is null or empty", nameof(file));
        }

        await using var stream = file.OpenReadStream();
        var fileId = await gridFsBucket.UploadFromStreamAsync(file.FileName, stream);

        return new UploadFileResponseDto(fileId.ToString());
    }

    /// <inheritdoc />
    public async Task<DownloadFileResponseDto?> GetFileById(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
        {
            return null;
        }

        try
        {
            var fileStream = await gridFsBucket.OpenDownloadStreamAsync(objectId);
            if (fileStream == null)
            {
                return null;
            }

            return new DownloadFileResponseDto
            (
                fileStream,
                "application/octet-stream",
                fileStream.FileInfo.Filename
            );
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving the file.", ex);
        }
    }
}