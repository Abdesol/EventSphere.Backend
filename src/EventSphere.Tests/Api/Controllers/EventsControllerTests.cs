using EventSphere.Api.Controllers;
using EventSphere.Application.Services.Interfaces;
using EventSphere.Infrastructure.Security;
using Moq;

namespace EventSphere.Tests.Api.Controllers;

public class EventsControllerTests
{
    private readonly EventsController _controller;
    private readonly Mock<IAccountService> _mockAccountService;
    private readonly Mock<IEventService> _mockEventService;
    private readonly Mock<JwtHandler> _mockJwtHandler;
}