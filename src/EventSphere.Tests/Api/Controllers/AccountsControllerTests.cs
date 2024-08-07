using EventSphere.Api.Controllers;
using EventSphere.Application.Services.Interfaces;
using EventSphere.Common.Utilities;
using EventSphere.Domain.Dtos;
using EventSphere.Domain.Entities;
using EventSphere.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace EventSphere.Tests.Api.Controllers;

public class AccountsControllerTests
{
    private readonly AccountsController _controller;
    private readonly Mock<IAccountService> _mockAccountService;
    private readonly Mock<ITokenBlacklistService> _mockTokenBlacklistService;
    private readonly Mock<IConfiguration> _mockConfiguration;

    public AccountsControllerTests()
    {
        _mockAccountService = new Mock<IAccountService>();
        _mockTokenBlacklistService = new Mock<ITokenBlacklistService>();
        _mockConfiguration = new Mock<IConfiguration>();

        var mockJwtExpiryInMinutesConfig = new Mock<IConfigurationSection>();
        mockJwtExpiryInMinutesConfig.Setup(x => x.Value).Returns("60");
        _mockConfiguration.Setup(x => x.GetSection("Jwt:ExpiryInMinutes")).Returns(mockJwtExpiryInMinutesConfig.Object);

        _controller = new AccountsController(
            _mockAccountService.Object,
            _mockTokenBlacklistService.Object,
            _mockConfiguration.Object
        );
    }

    [Fact]
    public void Register_WhenUsernameIsAlreadyTaken_ReturnsBadRequest()
    {
        var userDto = new UserRegistrationRequestDto(
            "test@gmail.com",
            "test",
            "Test1234####"
        );
        _mockAccountService.Setup(x => x.Register(userDto, false, ""));

        _mockAccountService.Setup(x => x.IsThereSimilarUsernames(userDto.Username))
            .ReturnsAsync(true);

        var result = _controller.Register(userDto).Result;

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public void Register_WhenEmailIsAlreadyTaken_ReturnsUnauthorized()
    {
        var userDto = new UserRegistrationRequestDto(
            "test@gmail.com",
            "test",
            "Test1234####"
        );
        _mockAccountService.Setup(x => x.Register(userDto, false, ""));

        _mockAccountService.Setup(x => x.IsThereSimilarEmails(userDto.Email))
            .ReturnsAsync(true);

        var result = _controller.Register(userDto).Result;

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public void Register_WhenUserIsRegistered_ReturnsCreated()
    {
        var userDto = new UserRegistrationRequestDto(
            "test@gmail.com",
            "test",
            "Test1234####"
        );

        var registeredUser = new User
        {
            Id = 0,
            Email = userDto.Email,
            Username = userDto.Username,
            Role = Role.User,
            PasswordHash = HashHelper.Hash(userDto.Password)
        };

        _mockAccountService.Setup(service => service.Register(userDto, false, ""))
            .ReturnsAsync(registeredUser);

        var result = _controller.Register(userDto).Result;

        var createdResult = Assert.IsType<CreatedResult>(result);
        Assert.Equal(201, createdResult.StatusCode);
    }

    [Fact]
    public void Authenticate_WhenUserIsRegisteredWithOAuth_ReturnsConflict()
    {
        var userDto = new UserAuthenticationRequestDto("test@gmail.com", "");
        _mockAccountService.Setup(x => x.IsUserRegisteredWithOAuth(userDto.Email))
            .ReturnsAsync(true);

        var result = _controller.Authenticate(userDto).Result;
        var conflictResult = Assert.IsType<ConflictResult>(result);
        Assert.Equal(409, conflictResult.StatusCode);
    }

    [Fact]
    public void Authenticate_WhenUserProvidedWrongCredentials_ReturnsUnauthorized()
    {
        var userDto = new UserAuthenticationRequestDto("test@gmail.com", "");
        
        _mockAccountService.Setup(x => x.IsUserRegisteredWithOAuth(userDto.Email))
            .ReturnsAsync(false);
        
        _mockAccountService.Setup(x => x.Authenticate(userDto, false))
            .ReturnsAsync((UserAuthenticationResponseDto?)null);
        
        var result = _controller.Authenticate(userDto).Result;
        var unauthorizedResult = Assert.IsType<UnauthorizedResult>(result);
        Assert.Equal(401, unauthorizedResult.StatusCode);
    }

    [Fact]
    public void Authenticate_WhenUserProvidedCorrectCredentials_ReturnsOk()
    {
        var userDto = new UserAuthenticationRequestDto("test@gmail.com", "");
        var userResponseDto = new UserAuthenticationResponseDto("Token", true);

        _mockAccountService.Setup(x => x.IsUserRegisteredWithOAuth(userDto.Email))
            .ReturnsAsync(false);

        _mockAccountService.Setup(x => x.Authenticate(userDto, false))
            .ReturnsAsync(userResponseDto);

        var result = _controller.Authenticate(userDto).Result;
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }
    
    [Fact]
    public void Logout_WhenUserIsAuthenticated_ReturnsOk()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Authorization = "Bearer Token";
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
        
        var result = _controller.Logout();
        var okResult = Assert.IsType<OkResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }
}