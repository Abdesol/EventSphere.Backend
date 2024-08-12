using EventSphere.Application.Services.Interfaces;
using EventSphere.Common.Enums;
using EventSphere.Common.Utilities;
using EventSphere.Domain.Dtos;
using EventSphere.Domain.Entities;
using EventSphere.Infrastructure.Data;
using EventSphere.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace EventSphere.Application.Services;

public class AccountService(JwtHandler jwtHandler, ApplicationDbContext appDbContext) : IAccountService
{
    /// <inheritdoc />
    public async Task<bool> IsThereSimilarUsernames(string username)
    {
        var user = await appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username);
        return user is not null;
    }
    
    /// <inheritdoc />
    public async Task<bool> IsThereSimilarEmails(string email)
    {
        var user = await appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
        return user is not null;
    }
    
    /// <inheritdoc />
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
    
    /// <inheritdoc />
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
        return new UserAuthenticationResponseDto(token, true, user.Id);
    }

    
    /// <inheritdoc />
    public async Task<string?> GenerateTokenByUserId(int id)
    {
        var user = await appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (user is null)
            return null;
        
        var token = jwtHandler.CreateToken(user);
        return token;
    }

    /// <inheritdoc />
    public async Task<User?> GetUserByEmail(string email)
    {
        return await appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
    }

    /// <inheritdoc />
    public async Task<User?> GetUserById(int id)
    {
        return await appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }
    
    /// <inheritdoc />
    public async Task<bool> IsUserRegisteredWithOAuth(string email)
    {
        var user = await appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
        return user is not null && user.IsOAuth;
    }
    
    /// <inheritdoc />
    public async Task<bool> DoesUserExist(int id)
    {
        var user = await appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return user is not null;
    }

    /// <inheritdoc />
    public async Task<bool> IsUserAlreadyAnEventOrganizer(int id)
    {
        var user = await appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return user is not null && user.Role == Role.EventOrganizer;
    }

    /// <inheritdoc />
    public async Task<bool> PromoteToEventOrganizer(int id)
    {
        var user = await appDbContext.Users.FindAsync(id);
        if (user == null) return false;
        
        user.Role = Role.EventOrganizer;
        appDbContext.Users.Update(user);
        await appDbContext.SaveChangesAsync();
        
        return true;
    }
}