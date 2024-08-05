namespace EventSphere.Application.Services.Interfaces;

public interface ICacheService
{
    void Set(string key, object value, TimeSpan expiration);
    
    T Get<T>(string key);
    
    void Remove(string key);
}