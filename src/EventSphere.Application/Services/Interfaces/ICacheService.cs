namespace EventSphere.Application.Services.Interfaces;

/// <summary>
/// Interface for a cache service.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Set a value in the cache.
    /// </summary>
    /// <param name="key">
    /// The key to store the value under.
    /// </param>
    /// <param name="value">
    /// The value to store in the cache.
    /// </param>
    /// <param name="expiration">
    /// The expiration time for the cache entry.
    /// </param>
    void Set(string key, object value, TimeSpan expiration);
    
    /// <summary>
    /// Get a value from the cache.
    /// </summary>
    /// <param name="key">
    /// The key to retrieve the value from.
    /// </param>
    /// <typeparam name="T">
    /// The type of the value to retrieve.
    /// </typeparam>
    /// <returns>
    /// The value stored in the cache.
    /// </returns>
    T Get<T>(string key);
    
    /// <summary>
    /// Remove a value from the cache.
    /// </summary>
    /// <param name="key">
    /// The key to remove the value from.
    /// </param>
    void Remove(string key);
}