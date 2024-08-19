using EventSphere.Application.Services.Interfaces;
using EventSphere.Domain.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace EventSphere.Application.Services;

public class FileService(IConfiguration configuration, IGridFSBucket gridFsBucket, ICacheService cacheService)
    : IFileService
{
    private readonly TimeSpan _pendingFileDeleteExpiration =
        TimeSpan.FromMinutes(configuration.GetValue<int>("FileSettings:ExpiryInMinutes"));

    /// <inheritdoc />
    public async Task<UploadFileResponseDto> SaveFile(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is null or empty", nameof(file));
        }

        await using var stream = file.OpenReadStream();
        var fileId = await gridFsBucket.UploadFromStreamAsync(file.FileName, stream);

        AddFileToPendingToDispose(fileId.ToString());

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

    private void AddFileToPendingToDispose(string id)
    {
        var cacheKey = $"pending_file_{id}";
        cacheService.Set(cacheKey, id, 2 * _pendingFileDeleteExpiration);

        var cancellationTokenSource = new CancellationTokenSource();
        cacheService.Set($"cancel_token_{id}", cancellationTokenSource, 2 * _pendingFileDeleteExpiration);

        Task.Run(async () =>
        {
            try
            {
                await Task.Delay(_pendingFileDeleteExpiration, cancellationTokenSource.Token);

                if (cacheService.Get<string?>(cacheKey) is not null)
                {
                    cacheService.Remove(id);

                    var objectId = new ObjectId(id);
                    await gridFsBucket.DeleteAsync(objectId);
                }
            }
            catch (TaskCanceledException)
            {
                // Task was cancelled, no need to delete the file
            }
        });
    }

    /// <inheritdoc />
    public void FileIsUsed(string id)
    {
        cacheService.Remove($"pending_file_{id}");

        var cancellationTokenSource = cacheService.Get<CancellationTokenSource?>($"cancel_token_{id}");
        if (cancellationTokenSource is null) return;

        cancellationTokenSource.Cancel();
        cacheService.Remove($"cancel_token_{id}");
    }

    /// <inheritdoc />
    public async Task<bool> DoesFileExist(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
        {
            return false;
        }

        try
        {
            var fileStream = await gridFsBucket.OpenDownloadStreamAsync(objectId);
            return fileStream != null;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> IsFileTypeOfImage(string id)
    {
        var doesItExist = await DoesFileExist(id);
        if (!doesItExist)
        {
            return false;
        }

        if (!ObjectId.TryParse(id, out var objectId))
        {
            return false;
        }

        var fileStream = await gridFsBucket.OpenDownloadStreamAsync(objectId);
        var fileInfo =
            await (await gridFsBucket.FindAsync(Builders<GridFSFileInfo<ObjectId>>.Filter.Eq("_id", objectId)))
                .FirstOrDefaultAsync();
        if (fileInfo?.Metadata?.Contains("contentType") == true)
        {
            var contentType = fileInfo.Metadata["contentType"].AsString;
            return IsImageContentType(contentType);
        }

        // If no metadata, check the file header (magic numbers)
        var buffer = new byte[8];
        await fileStream.ReadAsync(buffer, 0, buffer.Length);

        return IsImageFileHeader(buffer);
    }

    private static bool IsImageContentType(string contentType)
    {
        var imageContentTypes = new HashSet<string>
        {
            "image/jpeg",
            "image/png",
        };

        return imageContentTypes.Contains(contentType);
    }

    private static bool IsImageFileHeader(IReadOnlyList<byte> header)
    {
        switch (header.Count)
        {
            // JPEG (FF D8 FF)
            case >= 3 when header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF:
            // PNG (89 50 4E 47 0D 0A 1A 0A)
            case >= 8 when header.Take(8).SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }):
                return true;
            default:
                return false;
        }
    }
}