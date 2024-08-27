using EventSphere.Domain.Dtos;
using EventSphere.Domain.Entities;

namespace EventSphere.Application.Services.Interfaces;

/// <summary>
/// The comment service interface
/// </summary>
public interface ICommentService
{
    /// <summary>
    /// Get comments for the event
    /// </summary>
    /// <param name="eventId">The id of the event</param>
    /// <returns>A list of comments if successful, otherwise null</returns>
    public Task<List<Comment>> GetComments(int eventId);
    
    /// <summary>
    /// Create a comment for the event
    /// </summary>
    /// <param name="commentRequestDto">comment request data transfer object</param>
    /// <param name="userId">The user id who created the comment</param>
    /// <returns>The comment object</returns>
    public Task<Comment> Create(CommentRequestDto commentRequestDto, int userId);
    
    /// <summary>
    /// Delete the comment from the event
    /// </summary>
    /// <param name="id">Id of the comment</param>
    /// <param name="eventId">Id of the event</param>
    /// <returns>true if successful, otherwise false</returns>
    public Task<bool> Delete(int id, int eventId);
    
    /// <summary>
    /// Update the comment
    /// </summary>
    /// <param name="commentUpdateRequestDto">The comment update request data transfer object</param>
    /// <returns>true if successful, otherwise false</returns>
    public Task<bool> Update(CommentUpdateRequestDto commentUpdateRequestDto);
    
    /// <summary>
    /// Returns a comment entity based on the id
    /// </summary>
    /// <param name="id">Id of the comment</param>
    /// <returns>The comment object if it exists, otherwise null</returns>
    public Task<Comment?> GetCommentById(int id);
}