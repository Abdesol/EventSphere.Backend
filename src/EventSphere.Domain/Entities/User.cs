using EventSphere.Domain.Enums;

namespace EventSphere.Domain.Entities;

public class User
{
    public int Id { get; set; }
    
    public string? Email { get; set; }
    
    public string Role { get; set; }
}