using System.ComponentModel.DataAnnotations;
using EventSphere.Common.Attributes;

namespace EventSphere.Domain.Dtos;

public record EventCreateRequestDto(
    [Required] [MinLength(5)] [MaxLength(50)] string Title,
    [Required] [MinLength(5)] [MaxLength(1000)] string Description,
    [Required] [MinLength(5)] [MaxLength(50)] string Location,
    [Required] [Range(0, double.PositiveInfinity)] int OwnerId,
    [Required] [DateNotInPast] DateOnly Date,
    [Required] [ValidUnixTimestamp] long StartTime,
    [Required] [ValidUnixTimestamp] long EndTime,
    List<string>? EventTypes = null!);