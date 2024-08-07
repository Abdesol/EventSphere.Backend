using EventSphere.Domain.Enums;

namespace EventSphere.Domain.Entities;

/// <summary>
/// The user database entity object
/// </summary>
public class User
{
    public int Id { get; set; }
    
    public string? Username { get; set; }
    
    public string? Email { get; set; }

    /// <summary>
    /// Role of the user in the system: User, EventOrganizer or Admin.
    /// </summary>
    public string Role { get; set; } = Enums.Role.User;
    
    /// <summary>
    /// The password hash of the user
    /// </summary>
    public string? PasswordHash { get; set; }
    
    /// <summary>
    /// A boolean value to determine if the user is an OAuth user
    /// </summary>
    public bool IsOAuth { get; set; }
    
    /// <summary>
    /// If the user is OAuth authenticated user, this field will contain the OAuth client name
    /// </summary>
    public string? OAuthClient { get; set; }
}