using EventSphere.Api.Controllers;
using EventSphere.Application.Services.Interfaces;
using EventSphere.Common.Utilities;
using EventSphere.Domain.Dtos;
using EventSphere.Domain.Entities;
using EventSphere.Common.Enums;
using EventSphere.Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Moq;

namespace EventSphere.Tests.Api.Controllers;

public class AccountsControllerTests
{
    private readonly AccountsController _controller;
    private readonly Mock<IAccountService> _mockAccountService;
    private readonly Mock<IFileService> _mockFileService;
    private readonly Mock<JwtHandler> _mockJwtHandler;

    public AccountsControllerTests()
    {
        _mockAccountService = new Mock<IAccountService>();
        _mockFileService = new Mock<IFileService>();
        var mockTokenBlacklistService = new Mock<ITokenBlacklistService>();
        var mockConfiguration = new Mock<IConfiguration>();

        var mockJwtExpiryInMinutesConfig = new Mock<IConfigurationSection>();
        mockJwtExpiryInMinutesConfig.Setup(x => x.Value).Returns("60");
        mockConfiguration.Setup(x => x.GetSection("JwtSettings:ExpiryInMinutes")).Returns(mockJwtExpiryInMinutesConfig.Object);

        var mockJwtSection = new Mock<IConfigurationSection>();
        mockConfiguration.Setup(x => x.GetSection("JwtSettings")).Returns(mockJwtSection.Object);
        _mockJwtHandler = new Mock<JwtHandler>(mockConfiguration.Object);
        
        var httpContext = new DefaultHttpContext
        {
            Request =
            {
                Scheme = "http",
                Host = new HostString("localhost")
            }
        };

        _controller = new AccountsController(
            _mockAccountService.Object,
            _mockFileService.Object,
            mockTokenBlacklistService.Object,
            mockConfiguration.Object,
            _mockJwtHandler.Object
        )
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };
    }
    
    [Fact]
    public void GetUser_WhenEventIsNotFound_ReturnsBadRequest()
    {
        _mockAccountService.Setup(x => x.GetUserById(It.IsAny<int>()))
            .ReturnsAsync((User)null!);

        var result = _controller.GetUser(It.IsAny<int>()).Result;

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public void GetUser_WhenEventIsFound_ReturnsOk()
    {
        _mockAccountService.Setup(x => x.GetUserById(It.IsAny<int>()))
            .ReturnsAsync(new User());

        var result = _controller.GetUser(It.IsAny<int>()).Result;

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
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
        var userResponseDto = new UserAuthenticationResponseDto("Token", true, 0);

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
    
    [Fact]
    public async void PromoteToEventOrganizer_WhenUserIsAlreadyAnEventOrganizer_ReturnsBadRequest()
    {
        _mockAccountService.Setup(x => x.DoesUserExist(It.IsAny<int>()))
            .ReturnsAsync(true);
        _mockAccountService.Setup(x => x.IsUserAlreadyAnEventOrganizer(It.IsAny<int>()))
            .ReturnsAsync(true);
        _mockAccountService.Setup(x => x.GetUserByEmail(It.IsAny<string>()))
            .ReturnsAsync(new User());
        _mockJwtHandler.Setup(x => x.GetUserEmail(It.IsAny<StringValues>()))
            .Returns("test@gmail.com");

        var result = await _controller.PromoteToEventOrganizer();
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }
    
    [Fact]
    public async void PromoteToEventOrganizer_WhenUpdateRoleIsNotSuccessful_ReturnsUnprocessableEntity()
    {
        _mockAccountService.Setup(x => x.DoesUserExist(It.IsAny<int>()))
            .ReturnsAsync(true);
        _mockAccountService.Setup(x => x.IsUserAlreadyAnEventOrganizer(It.IsAny<int>()))
            .ReturnsAsync(false);
        _mockAccountService.Setup(x => x.PromoteToEventOrganizer(It.IsAny<int>()))
            .ReturnsAsync(false);
        _mockAccountService.Setup(x => x.GetUserByEmail(It.IsAny<string>()))
            .ReturnsAsync(new User());
        _mockJwtHandler.Setup(x => x.GetUserEmail(It.IsAny<StringValues>()))
            .Returns("test@gmail.com");

        var result = await _controller.PromoteToEventOrganizer();
        var unprocessableEntityResult = Assert.IsType<UnprocessableEntityObjectResult>(result);
        Assert.Equal(422, unprocessableEntityResult.StatusCode);
    }
    
    [Fact]
    public async void PromoteToEventOrganizer_WhenNotAbleToCreateNewToken_ReturnsUnprocessableEntity()
    {
        _mockAccountService.Setup(x => x.DoesUserExist(It.IsAny<int>()))
            .ReturnsAsync(true);
        _mockAccountService.Setup(x => x.IsUserAlreadyAnEventOrganizer(It.IsAny<int>()))
            .ReturnsAsync(false);
        _mockAccountService.Setup(x => x.PromoteToEventOrganizer(It.IsAny<int>()))
            .ReturnsAsync(true);
        _mockAccountService.Setup(x => x.GenerateTokenByUserId(It.IsAny<int>()))
            .ReturnsAsync((string?)null);
        _mockAccountService.Setup(x => x.GetUserByEmail(It.IsAny<string>()))
            .ReturnsAsync(new User());
        _mockJwtHandler.Setup(x => x.GetUserEmail(It.IsAny<StringValues>()))
            .Returns("test@gmail.com");

        var result = await _controller.PromoteToEventOrganizer();
        var unprocessableEntityResult = Assert.IsType<UnprocessableEntityObjectResult>(result);
        Assert.Equal(422, unprocessableEntityResult.StatusCode);
    }
    
    [Fact]
    public async void SetProfilePicture_WhenUserIsNotAuthenticated_ReturnsUnauthorized()
    {
        _mockJwtHandler.Setup(x => x.GetUserEmail(It.IsAny<StringValues>()))
            .Returns((string?)null);

        var result = await _controller.SetProfilePicture(It.IsAny<string>());
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(401, unauthorizedResult.StatusCode);
    }
    
    [Fact]
    public async void SetProfilePicture_WhenProfilePictureFileDoesNotExist_ReturnsBadRequest()
    {
        _mockJwtHandler.Setup(x => x.GetUserEmail(It.IsAny<StringValues>()))
            .Returns("test@gmail.com");
        _mockAccountService.Setup(x => x.GetUserByEmail(It.IsAny<string>()))
            .ReturnsAsync(new User());
        _mockFileService.Setup(x => x.DoesFileExist(It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = await _controller.SetProfilePicture(It.IsAny<string>());
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }
    
    [Fact]
    public async void SetProfilePicture_WhenProfilePictureFileIsNotImage_ReturnsBadRequest()
    {
        _mockJwtHandler.Setup(x => x.GetUserEmail(It.IsAny<StringValues>()))
            .Returns("test@gmail.com");
        _mockAccountService.Setup(x => x.GetUserByEmail(It.IsAny<string>()))
            .ReturnsAsync(new User());
        _mockFileService.Setup(x => x.DoesFileExist(It.IsAny<string>()))
            .ReturnsAsync(true);
        _mockFileService.Setup(x => x.IsFileTypeOfImage(It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = await _controller.SetProfilePicture(It.IsAny<string>());
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }
    
    [Fact]
    public async void SetProfilePicture_WhenProfilePictureIsNotSet_ReturnsInternalServerError()
    {
        _mockJwtHandler.Setup(x => x.GetUserEmail(It.IsAny<StringValues>()))
            .Returns("test@gmail.com");
        _mockAccountService.Setup(x => x.GetUserByEmail(It.IsAny<string>()))
            .ReturnsAsync(new User());
        _mockFileService.Setup(x => x.DoesFileExist(It.IsAny<string>()))
            .ReturnsAsync(true);
        _mockFileService.Setup(x => x.IsFileTypeOfImage(It.IsAny<string>()))
            .ReturnsAsync(true);
        _mockAccountService.Setup(x => x.SetProfilePicture(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = await _controller.SetProfilePicture(It.IsAny<string>());
        var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, internalServerErrorResult.StatusCode);
    }
}