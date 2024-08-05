using EventSphere.Application.Services.Interfaces;

namespace EventSphere.Application.Services;

public class TokenBlacklistService(ICacheService cacheService) : ITokenBlacklistService
{
    public void BlacklistToken(string token, TimeSpan expiration)
    {
        var key = $"blacklist_{token}";
        cacheService.Set(key, true, expiration);
    }

    public bool IsTokenBlacklisted(string token)
    {
        var key = $"blacklist_{token}";
        return cacheService.Get<bool>(key);
    }
}