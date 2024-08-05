namespace EventSphere.Application.Services.Interfaces;

public interface ITokenBlacklistService
{
    public void BlacklistToken(string token, TimeSpan expiration);
    
    public bool IsTokenBlacklisted(string token);
}