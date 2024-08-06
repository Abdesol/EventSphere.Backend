using EventSphere.Domain.Dtos;
using EventSphere.Domain.Entities;

namespace EventSphere.Application.Services.Interfaces;

public interface IAccountService
{
    public Task<bool> IsThereSimilarUsernames(string username);
    
    public Task<bool> IsThereSimilarEmails(string email);
    
    public Task<User> Register(UserRegistrationRequestDto userRegistrationRequestDto, bool isOAuth = false, string oAuthClient = "");
    
    public Task<UserAuthenticationResponseDto?> Authenticate(UserAuthenticationRequestDto userAuthenticationRequestDto, bool isOAuth = false);
    
    public Task<User?> GetUserByEmail(string email);

    public Task<bool> IsUserRegisteredWithOAuth(string email);
}