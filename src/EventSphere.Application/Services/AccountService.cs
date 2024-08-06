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
    
    public async Task<User> Register(UserRegistrationRequestDto userRegistrationRequestDto, bool isOAuth = false, string oAuthClient = "")
    {
        var user = new User
        {
            Username = userRegistrationRequestDto.Username,
            Email = userRegistrationRequestDto.Email,
            Role = userRegistrationRequestDto.IsEventOrganizer ? Role.EventOrganizer : Role.User,
            IsOAuth = isOAuth
        };

        if (!isOAuth)
        {
            user.PasswordHash = HashHelper.Hash(userRegistrationRequestDto.Password);
        }
        else
        {
            user.OAuthClient = oAuthClient;
        }

        var userEntry = await appDbContext.Users.AddAsync(user);
        await appDbContext.SaveChangesAsync();

        return userEntry.Entity;
    }
    
    public async Task<UserAuthenticationResponseDto?> Authenticate(UserAuthenticationRequestDto userAuthenticationRequestDto, bool isOAuth = false)
    {
        var user = await appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == userAuthenticationRequestDto.Email);
        if (user == null)
        {
            return null;
        }

        if (!isOAuth)
        {
            var isPasswordValid = HashHelper.Verify(userAuthenticationRequestDto.Password, user.PasswordHash!);
            if (!isPasswordValid)
            {
                return null;
            }
        }

        var token = jwtHandler.CreateToken(user);
        return new UserAuthenticationResponseDto(token, true);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
    }
    
    public async Task<bool> IsUserRegisteredWithOAuth(string email)
    {
        var user = await appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
        return user != null && user.IsOAuth;
    }
}