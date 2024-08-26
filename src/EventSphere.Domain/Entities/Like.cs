namespace EventSphere.Domain.Entities;

/// <summary>
/// The like database object for the events
/// </summary>
public class Like
{
    /// <summary>
    /// The id of the event that is liked
    /// </summary>
    public int EventId { get; set; }
    
    /// <summary>
    /// The user who liked the specific event
    /// </summary>
    public int UserId { get; set; }
}