using EventSphere.Application.Services;
using EventSphere.Application.Services.Interfaces;
using EventSphere.Common.Enums;
using EventSphere.Domain.Dtos;
using EventSphere.Domain.Entities;
using EventSphere.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EventSphere.Tests.Application.Services;

public class CommentServiceTests
{
    private readonly ICommentService _service;
    private readonly ApplicationDbContext _appDbContext;

    public CommentServiceTests()
    {
        var uniqueDatabaseName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: uniqueDatabaseName)
            .Options;
        _appDbContext = new ApplicationDbContext(options);
        SeedDatabase();

        var cache = new MemoryCache(new MemoryCacheOptions());
        _service = new CommentService(cache, _appDbContext);
    }

    private void SeedDatabase()
    {
        _appDbContext.Events.Add(
            new Event()
            {
                Id = 1,
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
        _appDbContext.Comments.Add(
            new Comment()
            {
                Id = 1,
                EventId = 1,
                UserId = 0,
                Content = "Test Comment 1",
                CreatedAt = new DateTimeOffset(new DateTime(2024, 01, 01)).ToUnixTimeSeconds(),
                UpdatedAt = new DateTimeOffset(new DateTime(2024, 01, 01)).ToUnixTimeSeconds()
            });
        _appDbContext.SaveChanges();
    }

    [Fact]
    public void Update_ShouldUpdateTheUpdatedAtProperty()
    {
        var commentUpdateRequestDto = new CommentUpdateRequestDto(1, 1, "Updated Comment");
        
        var result = _service.Update(commentUpdateRequestDto).Result;
        Assert.True(result);
        
        var comment = _appDbContext.Comments.Find(1);
        Assert.NotEqual(comment!.CreatedAt, comment.UpdatedAt);
    }

    [Fact]
    public void Update_ShouldUpdateTheContentProperty()
    {
        var commentUpdateRequestDto = new CommentUpdateRequestDto(1, 1, "Updated Comment");
        
        var result = _service.Update(commentUpdateRequestDto).Result;
        Assert.True(result);
        
        var comment = _appDbContext.Comments.Find(1);
        Assert.Equal(commentUpdateRequestDto.Content, comment!.Content);
    }
}