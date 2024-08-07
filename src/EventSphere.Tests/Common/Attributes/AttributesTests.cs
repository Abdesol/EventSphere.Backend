using EventSphere.Common.Attributes;

namespace EventSphere.Tests.Common.Attributes;

public class AttributesTests
{
    [Fact]
    public void AlphanumericAttribute_ShouldReturnTrue_WhenValueIsAlphanumeric()
    {
        var alphanumericAttribute = new AlphanumericAttribute();
        var result = alphanumericAttribute.IsValid("Alphanumeric123");
        Assert.True(result);
    }
    
    [Fact]
    public void AlphanumericAttribute_ShouldReturnFalse_WhenValueIsNotAlphanumeric()
    {
        var alphanumericAttribute = new AlphanumericAttribute();
        var result = alphanumericAttribute.IsValid("Alphanumeric123!");
        Assert.False(result);
    }
}