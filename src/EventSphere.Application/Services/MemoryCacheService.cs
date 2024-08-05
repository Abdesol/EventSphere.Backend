using EventSphere.Application.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace EventSphere.Application.Services;

public class MemoryCacheService(IMemoryCache cache) : ICacheService
{
    public void Set(string key, object value, TimeSpan expiration)
    {
        cache.Set(key, value, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        });
    }

    public T Get<T>(string key)
    {
        return cache.TryGetValue(key, out T value) ? value : default;
    }

    public void Remove(string key)
    {
        cache.Remove(key);
    }
}