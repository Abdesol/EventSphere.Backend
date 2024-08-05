using EventSphere.Application.Services.Interfaces;
using EventSphere.Common.Utilities;
using EventSphere.Domain.Dtos;
using EventSphere.Domain.Entities;
using EventSphere.Domain.Enums;
using EventSphere.Infrastructure.Data;
using EventSphere.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace EventSphere.Application.Services;

public class AccountService(JwtHandler jwtHandler, ApplicationDbContext appDbContext) : IAccountService
{
    public async Task<bool> IsThereSimilarUsernames(string username)
    {
        var user = await appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username);
        return user != null;
    }
    
    public async Task<bool> IsThereSimilarEmails(string email)
    {
        var user = await appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
        return user != null;
    }
    
    public async Task<User> Register(UserRegistrationRequestDto userRegistrationRequestDto)
    {
        var passwordHash = HashHelper.Hash(userRegistrationRequestDto.Password);

        var user = await appDbContext.Users.AddAsync(new User
        {
            Username = userRegistrationRequestDto.Username,
            Email = userRegistrationRequestDto.Email,
            PasswordHash = passwordHash,
            Role = userRegistrationRequestDto.IsEventOrganizer ? Role.EventOrganizer : Role.User
        });
        await appDbContext.SaveChangesAsync();

        return user.Entity;
    }
    
    public async Task<UserAuthenticationResponseDto?> Authenticate(UserAuthenticationRequestDto userAuthenticationRequestDto)
    {
        var user = await appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == userAuthenticationRequestDto.Email);
        if (user == null)
        {
            return null;
        }

        var isPasswordValid = HashHelper.Verify(userAuthenticationRequestDto.Password, user.PasswordHash!);
        if (!isPasswordValid)
        {
            return null;
        }

        var token = jwtHandler.CreateToken(user);
        return new UserAuthenticationResponseDto(token, true);
    }
}