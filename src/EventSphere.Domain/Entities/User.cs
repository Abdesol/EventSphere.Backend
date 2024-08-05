using EventSphere.Domain.Enums;

namespace EventSphere.Domain.Entities;

public class User
{
    public int Id { get; set; }
    
    public string? Username { get; set; }
    
    public string? Email { get; set; }

    public string Role { get; set; } = Enums.Role.User;
    
    public byte[] PasswordHash { get; set; } = null!;
    
    public byte[] PasswordSalt { get; set; } = null!;
}