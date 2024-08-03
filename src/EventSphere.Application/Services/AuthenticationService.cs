using EventSphere.Application.Services.Interfaces;
using EventSphere.Infrastructure.Security;

namespace EventSphere.Application.Services;

public class AuthenticationService(JwtHandler jwtHandler) : IAuthenticationService
{
    
}