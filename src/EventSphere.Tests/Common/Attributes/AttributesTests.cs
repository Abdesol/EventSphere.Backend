using EventSphere.Common.Attributes;

namespace EventSphere.Tests.Common.Attributes;

public class AttributesTests
{
    [Fact]
    public void AlphanumericAttribute_WhenValueIsAlphanumeric_ReturnsTrue()
    {
        var alphanumericAttribute = new AlphanumericAttribute();
        var result = alphanumericAttribute.IsValid("Alphanumeric123");
        Assert.True(result);
    }
    
    [Fact]
    public void AlphanumericAttribute_WhenValueIsNotAlphanumeric_ReturnsFalse()
    {
        var alphanumericAttribute = new AlphanumericAttribute();
        var result = alphanumericAttribute.IsValid("Alphanumeric123!");
        Assert.False(result);
    }
    
    [Fact]
    public void UnixTimeNotInPastOrFarFutureAttribute_WhenTimeIsInThePresent_ReturnsTrue()
    {
        var tomorrow = DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds();
        var dateNotInPastAttribute = new UnixTimeNotInPastOrFarFutureAttribute();
        var result = dateNotInPastAttribute.IsValid(tomorrow);
        Assert.True(result);
    }
    
    [Fact]
    public void UnixTimeNotInPastOrFarFutureAttribute_WhenTimeIsInThePast_ReturnsFalse()
    {
        var yesterday = DateTimeOffset.UtcNow.AddDays(-1).ToUnixTimeSeconds();
        var dateNotInPastAttribute = new UnixTimeNotInPastOrFarFutureAttribute();
        var result = dateNotInPastAttribute.IsValid(yesterday);
        Assert.False(result);
    }
    
    [Fact]
    public void UnixTimeNotInPastOrFarFutureAttribute_WhenTimeIsInInMoreThanAYear_ReturnsFalse()
    {
        var afterTwoYears = DateTimeOffset.UtcNow.AddYears(2).ToUnixTimeSeconds();
        var dateNotInPastAttribute = new UnixTimeNotInPastOrFarFutureAttribute();
        var result = dateNotInPastAttribute.IsValid(afterTwoYears);
        Assert.False(result);
    }
    
    [Fact]
    public void ValidEventTypesAttribute_WhenAllTheEventTypesAreInTheValidEventTypesList_ReturnsTrue()
    {
        var eventTypes = new List<string>()
        {
            "General", "Sport", "Technology"
        };
        var dateNotInPastAttribute = new ValidEventTypesAttribute();
        var result = dateNotInPastAttribute.IsValid(eventTypes);
        Assert.True(result);
    }
    
    [Fact]
    public void ValidEventTypesAttribute_WhenOneEventTypeIsNotInTheValidEventTypesList_ReturnsFalse()
    {
        var eventTypes = new List<string>()
        {
            "General", "InValidType", "Sport"
        };
        var dateNotInPastAttribute = new ValidEventTypesAttribute();
        var result = dateNotInPastAttribute.IsValid(eventTypes);
        Assert.False(result);
    }
}