using System.ComponentModel.DataAnnotations;
using EventSphere.Common.Attributes;

namespace EventSphere.Domain.Dtos;

public record EventCreateRequestDto(
    [Required] [MinLength(5)] [MaxLength(100)] string Title,
    [Required] [MinLength(5)] [MaxLength(1000)] string Description,
    [Required] [MinLength(5)] [MaxLength(50)] string Location,
    [Required] [ValidUnixTimestamp] [UnixTimeNotInPastOrFarFuture] long StartTime,
    [Required] [ValidUnixTimestamp] [UnixTimeNotInPastOrFarFuture] long EndTime,
    [ValidEventTypes] List<string>? EventTypes = null!,
    string? BannerPictureId = null);