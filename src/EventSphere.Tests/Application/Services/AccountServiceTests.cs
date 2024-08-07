using EventSphere.Application.Services;
using EventSphere.Application.Services.Interfaces;
using EventSphere.Common.Utilities;
using EventSphere.Domain.Dtos;
using EventSphere.Domain.Entities;
using EventSphere.Domain.Enums;
using EventSphere.Infrastructure.Data;
using EventSphere.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Moq;

namespace EventSphere.Tests.Application.Services;

public class AccountServiceTests
{
    private readonly IAccountService _service;
    private readonly Mock<JwtHandler> _mockJwtHandler;
    private readonly ApplicationDbContext _appDbContext;

    public AccountServiceTests()
    {
        var mockConfiguration = new Mock<IConfiguration>();
        var mockJwtSection = new Mock<IConfigurationSection>();
        mockConfiguration.Setup(x => x.GetSection("JwtSettings")).Returns(mockJwtSection.Object);
        _mockJwtHandler = new Mock<JwtHandler>(mockConfiguration.Object);
        
        _mockJwtHandler.Setup(x => x.CreateToken(It.IsAny<User>()))
            .Returns(string.Empty);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _appDbContext = new ApplicationDbContext(options);
        SeedDatabase();
        
        _service = new AccountService(_mockJwtHandler.Object, _appDbContext);
    }

    private void SeedDatabase()
    {
        _appDbContext.Users.AddRange(
            [
                new User()
                {
                    Id = 1, Email = "test1@gmail.com", Username = "test1", PasswordHash = HashHelper.Hash("Test1234##"),
                    Role = Role.User, IsOAuth = false, OAuthClient = null
                },
                new User()
                {
                    Id = 2, Email = "test2@gmail.com", Username = "test2", PasswordHash = null,
                    Role = Role.User, IsOAuth = true, OAuthClient = "Google"
                }
            ]
        );
        _appDbContext.SaveChanges();
    }
    
    [Fact]
    public async void Authenticate_WhenUserExists_ReturnsToken()
    {
        var result = await _service.Authenticate(new UserAuthenticationRequestDto("test1@gmail.com", "Test1234##"));
        Assert.NotNull(result);
    }

    [Fact]
    public async void Authenticate_WhenUserDoesNotExist_ReturnsNull()
    {
        var result = await _service.Authenticate(new UserAuthenticationRequestDto("test3@gmail.com", "Test1234##"));
        Assert.Null(result);    
    }

    [Fact]
    public async void Authenticate_WhenPasswordIsInvalid_ReturnsNull()
    {
        var result = await _service.Authenticate(new UserAuthenticationRequestDto("test1@gmail.com", "InvalidPassword"));
        Assert.Null(result);    
    }
}