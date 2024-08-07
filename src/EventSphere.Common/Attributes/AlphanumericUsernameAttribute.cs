using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace EventSphere.Common.Attributes;

/// <summary>
/// The alphanumeric attribute that can be used as a decorator on strings for string validation to alphanumeric format
/// </summary>
public partial class AlphanumericAttribute : ValidationAttribute
{
    private static readonly Regex AlphanumericRegex = MyRegex();

    public override bool IsValid(object value)
    {
        if (value is string username)
        {
            return AlphanumericRegex.IsMatch(username);
        }
        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be alphanumeric and contain only letters and numbers.";
    }

    /// <summary>
    /// Alphanumeric regex property.
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"^[a-zA-Z0-9]+$", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}