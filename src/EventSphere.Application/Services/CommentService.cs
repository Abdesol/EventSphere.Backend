using EventSphere.Application.Services.Interfaces;
using EventSphere.Domain.Dtos;
using EventSphere.Domain.Entities;
using EventSphere.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EventSphere.Application.Services;

public class CommentService(IMemoryCache cache, ApplicationDbContext appDbContext) : ICommentService
{
    private const string CommentsCachePrefix = "Comments_";
    private static readonly TimeSpan CommentsCacheExpirationInMinutes = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan CommentsCacheSlidingExpirationInMinutes = TimeSpan.FromMinutes(1);

    /// <inheritdoc />
    public async Task<List<Comment>> GetComments(int eventId)
    {
        var cacheKey = CommentsCachePrefix + eventId;

        if (cache.TryGetValue(cacheKey, out List<Comment>? cachedComments))
        {
            return cachedComments!;
        }

        var comments = await appDbContext.Comments.AsNoTracking().Where(c => c.EventId == eventId).ToListAsync();
        
        var userIds = comments.Select(c => c.UserId).Distinct().ToList();
        var users = await appDbContext.Users
            .Where(u => userIds.Contains(u.Id))
            .AsNoTracking()
            .ToDictionaryAsync(u => u.Id);
        
        foreach (var comment in comments)
        {
            if (users.TryGetValue(comment.UserId, out var user))
            {
                comment.User = user;
            }
        }

        cache.Set(cacheKey, comments, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CommentsCacheExpirationInMinutes,
            SlidingExpiration = CommentsCacheSlidingExpirationInMinutes
        });

        return comments;
    }

    /// <inheritdoc />
    public async Task<Comment> Create(CommentRequestDto commentRequestDto, int userId)
    {
        var timeNowInUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        
        var comment = new Comment()
        {
            EventId = commentRequestDto.EventId!.Value,
            UserId = userId,
            Content = commentRequestDto.Content,
            CreatedAt = timeNowInUnix,
            UpdatedAt = timeNowInUnix
        };
        
        var commentEntry = await appDbContext.Comments.AddAsync(comment);
        await appDbContext.SaveChangesAsync();
        
        var cacheKey = CommentsCachePrefix + commentRequestDto.EventId!.Value;
        cache.Remove(cacheKey);
        
        commentEntry.Entity.User = await appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        
        return commentEntry.Entity;
    }

    /// <inheritdoc />
    public async Task<bool> Delete(int id, int eventId)
    {
        var comment = await appDbContext.Comments.FindAsync(id);
        if (comment is null) return false;

        appDbContext.Comments.Remove(comment);
        await appDbContext.SaveChangesAsync();
        
        var cacheKey = CommentsCachePrefix + eventId;
        cache.Remove(cacheKey);

        return true;
    }

    /// <inheritdoc />
    public async Task<bool> Update(CommentUpdateRequestDto commentUpdateRequestDto)
    {
        var comment = await appDbContext.Comments.FindAsync(commentUpdateRequestDto.Id);
        if (comment is null) return false;
        
        var timeNowInUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        comment.Content = commentUpdateRequestDto.Content;
        comment.UpdatedAt = timeNowInUnix;
        
        await appDbContext.SaveChangesAsync();

        var cacheKey = CommentsCachePrefix + commentUpdateRequestDto.EventId;
        cache.Remove(cacheKey);
        
        return true;
    }

    /// <inheritdoc />
    public async Task<Comment?> GetCommentById(int id)
    {
        return await appDbContext.Comments.FindAsync(id);
    }
}