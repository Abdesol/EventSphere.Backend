using System.Net.Http.Headers;
using EventSphere.Domain.Entities;
using EventSphere.Domain.Enums;
using EventSphere.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Moq;

namespace EventSphere.Tests.Infrastructure.Security;

public class SecurityTests
{
    private readonly JwtHandler _jwtHandler;

    public SecurityTests()
    {
        var jwtSettings = new Dictionary<string, string>
        {
            { "JWT_SECRET_KEY", "KeyForTestsPurposeOnlyAndItNeedsToBe32OrMoreCharactersLong" },
            { "JwtSettings:Issuer", "YourIssuer" },
            { "JwtSettings:Audience", "YourAudience" },
            { "JwtSettings:ExpiryInMinutes", "60" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(jwtSettings!)
            .Build();

        _jwtHandler = new JwtHandler(configuration);
    }

    [Fact]
    public void GetUserEmailFromToken_ShouldReturnEmail()
    {
        var user = new User()
        {
            Id = 0,
            Username = "test",
            Email = "test@gmail.com",
            Role = Role.User,
        };

        var token = _jwtHandler.CreateToken(user);
        
        var authHeader = new AuthenticationHeaderValue("Bearer", token);
        var email = _jwtHandler.GetUserEmail(authHeader.ToString());
        
        Assert.Equal(user.Email, email);
    }
}