using EventSphere.Application.Services;
using EventSphere.Application.Services.Interfaces;
using EventSphere.Common.Utilities;
using EventSphere.Domain.Dtos;
using EventSphere.Domain.Entities;
using EventSphere.Domain.Enums;
using EventSphere.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using DateTimeOffset = System.DateTimeOffset;

namespace EventSphere.Tests.Application.Services;

public class EventServiceTests
{
    private readonly IEventService _service;
    private readonly ApplicationDbContext _appDbContext;

    public EventServiceTests()
    {
        var uniqueDatabaseName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: uniqueDatabaseName)
            .Options;
        _appDbContext = new ApplicationDbContext(options);
        SeedDatabase();

        _service = new EventService(_appDbContext);
    }

    private void SeedDatabase()
    {
        _appDbContext.Events.Add(
            new Event()
            {
                Id = 0,
                Title = "Test Event 1",
                Description = "Test Event 1 Description",
                Location = "Test Event 1 Location",
                EventTypes = new List<string> { EventTypes.General },
                OwnerId = 0,
                Date = new DateOnly(2024, 01, 01),
                StartTime = new DateTimeOffset(new DateTime(2024, 01, 01)).ToUnixTimeSeconds(),
                EndTime = new DateTimeOffset(new DateTime(2024, 01, 01)).ToUnixTimeSeconds()
            }
        );
        _appDbContext.SaveChanges();
    }

    [Fact]
    public async void Create_ShouldSetTheDatePropertyCorrectly()
    {
        var correctDate = new DateOnly(2024, 05, 05);

        var startTime =
            new DateTimeOffset(new DateTime(correctDate.Year, correctDate.Month, correctDate.Day, 6, 0, 0)
                .ToUniversalTime()).ToUnixTimeSeconds();

        var endTime =
            new DateTimeOffset(new DateTime(correctDate.Year, correctDate.Month, correctDate.Day, 18, 0, 0)
                .ToUniversalTime()).ToUnixTimeSeconds();

        var createEventDto = new EventCreateRequestDto(
            "Test Event 2",
            "Test Event 2 Description",
            "Test Event 2 Location",
            startTime,
            endTime,
            new List<string> { EventTypes.General }
        );

        var createdEvent = await _service.Create(createEventDto, 0);

        Assert.Equal(correctDate, createdEvent.Date);
    }

    [Fact]
    public async void Update_ReturnsFalse_WhenIdIsNotFound()
    {
        var updateEventDto = new EventUpdateRequestDto(
            10, // id that does not exist
            null, // no update to title
            null, // no update to description
            null, // no update to location
            null, // no update to start time
            null // no update to end time
        );

        var isSuccessful = await _service.Update(updateEventDto);

        Assert.False(isSuccessful);
    }

    [Fact]
    public async void Update_DoesNotUpdateNullProperties_And_OnlyUpdatesTheNonNullProperties()
    {
        const int eventId = 1;
        const string newTitle = "Test Event 1 - Update";
        const string newLocation = "Test Event 1 Location - Update";
        var eventEntity = (await _appDbContext.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == eventId))!;

        var updateEventDto = new EventUpdateRequestDto(
            eventId, // id that does not exist
            newTitle,
            null, // no update to description
            newLocation,
            null, // no update to start time
            null // no update to end time
        );

        var isSuccessful = await _service.Update(updateEventDto);
        Assert.True(isSuccessful);

        var updatedEventEntity = (await _appDbContext.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == eventId))!;

        Assert.NotEqual(updatedEventEntity.Title, eventEntity.Title);
        Assert.Equal(updatedEventEntity.Description, eventEntity.Description);
        Assert.NotEqual(updatedEventEntity.Location, eventEntity.Location);
        Assert.Equal(updatedEventEntity.StartTime, eventEntity.StartTime);
        Assert.Equal(updatedEventEntity.EndTime, eventEntity.EndTime);
        Assert.Equal(updatedEventEntity.Date, eventEntity.Date);
    }

    [Fact]
    public async void Update_ShouldUpdateTheDatePropertyCorrectly()
    {
        var correctDate = new DateOnly(2024, 10, 05);

        var startTime =
            new DateTimeOffset(new DateTime(correctDate.Year, correctDate.Month, correctDate.Day, 10, 30, 0)
                .ToUniversalTime()).ToUnixTimeSeconds();

        var endTime =
            new DateTimeOffset(new DateTime(correctDate.Year, correctDate.Month, correctDate.Day, 18, 0, 0)
                .ToUniversalTime()).ToUnixTimeSeconds();

        var updateEventDto = new EventUpdateRequestDto(
            1,
            null, // no update to title
            null, // no update to description
            null, // no update to location
            startTime,
            endTime
        );

        var isSuccessful = await _service.Update(updateEventDto);

        Assert.True(isSuccessful);

        Assert.Equal(correctDate,
            (await _appDbContext.Events.FirstOrDefaultAsync(e => e.Id == updateEventDto.Id))!.Date);
    }
}