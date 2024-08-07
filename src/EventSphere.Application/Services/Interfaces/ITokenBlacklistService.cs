namespace EventSphere.Application.Services.Interfaces;

/// <summary>
/// Service for managing blacklisted tokens.
/// </summary>
public interface ITokenBlacklistService
{
    /// <summary>
    /// Blacklists a token.
    /// </summary>
    /// <param name="token">
    /// The token to blacklist.
    /// </param>
    /// <param name="expiration">
    /// The duration for which the token should be blacklisted.
    /// </param>
    public void BlacklistToken(string token, TimeSpan expiration);
    
    /// <summary>
    /// Checks if a token is blacklisted.
    /// </summary>
    /// <param name="token">
    /// The token to check.
    /// </param>
    /// <returns>
    /// True if the token is blacklisted; otherwise, false.
    /// </returns>
    public bool IsTokenBlacklisted(string token);
}