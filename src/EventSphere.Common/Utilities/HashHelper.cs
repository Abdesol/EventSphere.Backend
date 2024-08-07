namespace EventSphere.Common.Utilities;

/// <summary>
/// Data hashing helper class
/// </summary>
public class HashHelper
{
    /// <summary>
    /// Hashes the input string
    /// </summary>
    /// <param name="input">
    /// The input string to hash
    /// </param>
    /// <returns>
    /// The hashed string
    /// </returns>
    public static string Hash(string input)
    {
        return BCrypt.Net.BCrypt.HashPassword(input);
    }

    /// <summary>
    /// Verifies the input string against the hashed value
    /// </summary>
    /// <param name="value">
    /// The input string to verify
    /// </param>
    /// <param name="hashedValue">
    /// The hashed value to verify against
    /// </param>
    /// <returns>
    /// True if the input string matches the hashed value, false otherwise
    /// </returns>
    public static bool Verify(string value, string hashedValue)
    {
        return BCrypt.Net.BCrypt.Verify(value, hashedValue);
    }
}