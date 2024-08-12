using System.ComponentModel.DataAnnotations;
using EventSphere.Common.Attributes;

namespace EventSphere.Domain.Dtos;

public record EventUpdateRequestDto(
    [Required] int? Id,
    [MinLength(5)] [MaxLength(100)] string? Title,
    [MinLength(5)] [MaxLength(1000)] string? Description,
    [MinLength(5)] [MaxLength(50)] string? Location,
    [ValidUnixTimestamp] [UnixTimeNotInPastOrFarFuture] long? StartTime,
    [ValidUnixTimestamp] [UnixTimeNotInPastOrFarFuture] long? EndTime,
    [ValidEventTypes] List<string>? EventTypes = null!);