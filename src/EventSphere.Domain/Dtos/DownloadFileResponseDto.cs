namespace EventSphere.Domain.Dtos;

public record DownloadFileResponseDto(
    Stream FileStream,
    string ContentType,
    string FileName);