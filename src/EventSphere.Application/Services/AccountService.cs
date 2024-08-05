using EventSphere.Application.Services.Interfaces;
using EventSphere.Domain.Dtos;
using EventSphere.Domain.Entities;
using EventSphere.Infrastructure.Data;
using EventSphere.Infrastructure.Security;

namespace EventSphere.Application.Services;

public class AccountService(JwtHandler jwtHandler, ApplicationDbContext appDbContext) : IAccountService
{
    public async Task<User> Register(UserRegistrationRequestDto userRegistrationRequestDto)
    {
        return new User();
    }
}