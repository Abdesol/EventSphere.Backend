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
    public void DateNotInPastAttribute_WhenDateIsInThePresent_ReturnsTrue()
    {
        var tomorrow = DateTime.Today.AddDays(1);
        var date = DateOnly.FromDateTime(tomorrow);
        var dateNotInPastAttribute = new DateNotInPastAttribute();
        var result = dateNotInPastAttribute.IsValid(date);
        Assert.True(result);
    }
    
    [Fact]
    public void DateNotInPastAttribute_WhenDateIsInThePast_ReturnsFalse()
    {
        var yesterday = DateTime.Today.AddDays(-1);
        var date = DateOnly.FromDateTime(yesterday);
        var dateNotInPastAttribute = new DateNotInPastAttribute();
        var result = dateNotInPastAttribute.IsValid(date);
        Assert.False(result);
    }
}