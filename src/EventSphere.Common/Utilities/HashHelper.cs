namespace EventSphere.Common.Utilities;

public class HashHelper
{
    public static string Hash(string input)
    {
        return BCrypt.Net.BCrypt.HashPassword(input);
    }

    public static bool Verify(string value, string hashedValue)
    {
        return BCrypt.Net.BCrypt.Verify(value, hashedValue);
    }
}