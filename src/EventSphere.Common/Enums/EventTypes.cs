using System.Reflection;

namespace EventSphere.Common.Enums;

/// <summary>
/// Enum of the events' types
/// </summary>
public static class EventTypes
{
    public const string General = "General";

    public const string Sport = "Sport";
    
    public const string Art = "Art";
    
    public const string Science = "Science";
    
    public const string Technology = "Technology";
    
    public const string Gaming = "Gaming";
    
    public const string Adventure = "Adventure";
    
    public static List<string> All()
    {
        var fieldInfos = typeof(EventTypes).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        return fieldInfos
            .Where(field => field.IsLiteral && !field.IsInitOnly)
            .Select(field => (string)field.GetValue(null)!)
            .ToList();
    }
}