using System.ComponentModel.DataAnnotations.Schema;

namespace EventSphere.Domain.Entities;

/// <summary>
/// The comment database object for the events
/// </summary>
public class Comment
{
    public int Id { get; set; }
    
    /// <summary>
    /// The id of the event that is liked
    /// </summary>
    public int EventId { get; set; }
    
    /// <summary>
    /// The user who liked the specific event
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// Content of the comment as a text
    /// </summary>
    public string Content { get; set; }
    
    /// <summary>
    /// The date and time the comment is created
    /// </summary>
    public long CreatedAt { get; set; }
    
    /// <summary>
    /// The date and time the comment is updated
    /// </summary>
    public long UpdatedAt { get; set; }
    
    /// <summary>
    /// The user object for the comment
    /// </summary>
    [NotMapped]
    public User? User { get; set; }
}