namespace EventSphere.Domain.Entities;

/// <summary>
/// The event database entity object
/// </summary>
public class Event
{
    public int Id { get; set; }
    
    /// <summary>
    /// Title of the event
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// Description of the event
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Location of the event
    /// </summary>
    public string? Location { get; set; }
    
    /// <summary>
    /// the types of the events
    /// </summary>
    public List<string>? EventTypes { get; set; }
    
    /// <summary>
    /// User id that created and owns the event
    /// </summary>
    public int OwnerId { get; set; }
    
    /// <summary>
    /// Event date
    /// </summary>
    public DateOnly Date { get; set; }
    
    /// <summary>
    /// Event starting time
    /// </summary>
    public long StartTime { get; set; }
    
    /// <summary>
    /// Event ending time
    /// </summary>
    public long EndTime { get; set; }
}