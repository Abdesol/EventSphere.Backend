using EventSphere.Api.Controllers;
using EventSphere.Application.Services.Interfaces;
using EventSphere.Domain.Dtos;
using EventSphere.Domain.Entities;
using EventSphere.Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Moq;

namespace EventSphere.Tests.Api.Controllers;

public class EventsControllerTests
{
    private readonly EventsController _controller;
    private readonly Mock<IAccountService> _mockAccountService;
    private readonly Mock<IEventService> _mockEventService;
    private readonly Mock<JwtHandler> _mockJwtHandler;

    public EventsControllerTests()
    {
        _mockAccountService = new Mock<IAccountService>();
        _mockEventService = new Mock<IEventService>();

        var mockConfiguration = new Mock<IConfiguration>();
        var mockJwtSection = new Mock<IConfigurationSection>();
        mockConfiguration.Setup(x => x.GetSection("JwtSettings")).Returns(mockJwtSection.Object);
        _mockJwtHandler = new Mock<JwtHandler>(mockConfiguration.Object);

        var mockHttpContext = new Mock<HttpContext>();
        var mockRequest = new Mock<HttpRequest>();
        var mockHeaders = new HeaderDictionary
        {
            { "Authorization", "Bearer your-mock-token" }
        };
        mockRequest.Setup(r => r.Headers).Returns(mockHeaders);
        mockHttpContext.Setup(ctx => ctx.Request).Returns(mockRequest.Object);

        _controller = new EventsController(
            _mockEventService.Object,
            _mockAccountService.Object,
            _mockJwtHandler.Object
        )
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            }
        };
    }

    [Fact]
    public void GetEvent_WhenEventIsNotFound_ReturnsBadRequest()
    {
        _mockEventService.Setup(x => x.GetEventById(It.IsAny<int>()))
            .ReturnsAsync((Event)null!);

        var result = _controller.GetEvent(It.IsAny<int>()).Result;

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public void GetEvent_WhenEventIsFound_ReturnsOk()
    {
        _mockEventService.Setup(x => x.GetEventById(It.IsAny<int>()))
            .ReturnsAsync(new Event());

        var result = _controller.GetEvent(It.IsAny<int>()).Result;

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public void Create_WhenUserEmailIsNotFound_ReturnsUnauthorized()
    {
        _mockJwtHandler.Setup(x => x.GetUserEmail(It.IsAny<StringValues>()))
            .Returns((string?)null);

        var result = _controller.Create(It.IsAny<EventCreateRequestDto>()).Result;

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(401, unauthorizedResult.StatusCode);
    }

    [Fact]
    public void Create_WhenUserIsNotFound_ReturnsUnauthorized()
    {
        _mockJwtHandler.Setup(x => x.GetUserEmail(It.IsAny<StringValues>()))
            .Returns("");
        _mockAccountService.Setup(x => x.GetUserByEmail(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        var result = _controller.Create(It.IsAny<EventCreateRequestDto>()).Result;

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(401, unauthorizedResult.StatusCode);
    }

    [Fact]
    public void Create_WhenEventIsCreated_ReturnsCreated()
    {
        _mockJwtHandler.Setup(x => x.GetUserEmail(It.IsAny<StringValues>()))
            .Returns("");
        _mockAccountService.Setup(x => x.GetUserByEmail(It.IsAny<string>()))
            .ReturnsAsync(new User());
        _mockEventService.Setup(x => x.Create(It.IsAny<EventCreateRequestDto>(), It.IsAny<int>()))
            .ReturnsAsync(new Event());

        var result = _controller.Create(It.IsAny<EventCreateRequestDto>()).Result;

        var createdResult = Assert.IsType<CreatedResult>(result);
        Assert.Equal(201, createdResult.StatusCode);
    }

    [Fact]
    public void Delete_WhenEventIsNotFound_ReturnsBadRequest()
    {
        _mockEventService.Setup(x => x.DoesEventExist(It.IsAny<int>()))
            .ReturnsAsync(false);

        var result = _controller.Delete(It.IsAny<int>()).Result;

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public void Delete_WhenNotAbleToDelete_ReturnsBadRequest()
    {
        _mockEventService.Setup(x => x.DoesEventExist(It.IsAny<int>()))
            .ReturnsAsync(true);
        _mockEventService.Setup(x => x.Delete(It.IsAny<int>()))
            .ReturnsAsync(false);

        var result = _controller.Delete(It.IsAny<int>()).Result;

        var badRequestResult = Assert.IsType<UnprocessableEntityObjectResult>(result);
        Assert.Equal(422, badRequestResult.StatusCode);
    }

    [Fact]
    public void Delete_WhenEventIsDeleted_ReturnsOk()
    {
        _mockEventService.Setup(x => x.DoesEventExist(It.IsAny<int>()))
            .ReturnsAsync(true);
        _mockEventService.Setup(x => x.Delete(It.IsAny<int>()))
            .ReturnsAsync(true);

        var result = _controller.Delete(It.IsAny<int>()).Result;

        var okResult = Assert.IsType<OkResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    private readonly EventUpdateRequestDto _testEventUpdateRequestDto = new(0, "test", "test", "test", 0, 0, null!);

    [Fact]
    public void Update_WhenEventIsNotFound_ReturnsBadRequest()
    {
        _mockEventService.Setup(x => x.DoesEventExist(It.IsAny<int>()))
            .ReturnsAsync(false);

        var result = _controller.Update(_testEventUpdateRequestDto).Result;

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public void Update_WhenNotAbleToUpdate_ReturnsBadRequest()
    {
        _mockEventService.Setup(x => x.DoesEventExist(It.IsAny<int>()))
            .ReturnsAsync(true);
        _mockEventService.Setup(x => x.Update(It.IsAny<EventUpdateRequestDto>()))
            .ReturnsAsync(false);

        var result = _controller.Update(_testEventUpdateRequestDto).Result;

        var badRequestResult = Assert.IsType<UnprocessableEntityObjectResult>(result);
        Assert.Equal(422, badRequestResult.StatusCode);
    }

    [Fact]
    public void Update_WhenEventIsUpdated_ReturnsOk()
    {
        _mockEventService.Setup(x => x.DoesEventExist(It.IsAny<int>()))
            .ReturnsAsync(true);
        _mockEventService.Setup(x => x.Update(It.IsAny<EventUpdateRequestDto>()))
            .ReturnsAsync(true);

        var result = _controller.Update(_testEventUpdateRequestDto).Result;

        var okResult = Assert.IsType<OkResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }
}