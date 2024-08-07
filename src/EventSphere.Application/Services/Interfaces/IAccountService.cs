using EventSphere.Domain.Dtos;
using EventSphere.Domain.Entities;

namespace EventSphere.Application.Services.Interfaces;

/// <summary>
/// The account service interface.
/// </summary>
public interface IAccountService
{
    /// <summary>
    /// Checks if there are similar usernames in the database.
    /// </summary>
    public Task<bool> IsThereSimilarUsernames(string username);
    
    /// <summary>
    /// Checks if there are similar emails in the database.
    /// </summary>
    public Task<bool> IsThereSimilarEmails(string email);
    
    /// <summary>
    /// Registers a user.
    /// </summary>
    /// <param name="userRegistrationRequestDto">
    /// The user registration request data transfer object.
    /// </param>
    /// <param name="isOAuth">
    /// A boolean value that indicates if the user is registered with OAuth.
    /// </param>
    /// <param name="oAuthClient">
    /// The OAuth client.
    /// </param>
    /// <returns>
    /// The user entity.
    /// </returns>
    public Task<User> Register(UserRegistrationRequestDto userRegistrationRequestDto, bool isOAuth = false, string oAuthClient = "");
    
    /// <summary>
    /// Authenticates a user.
    /// </summary>
    /// <param name="userAuthenticationRequestDto">
    /// The user authentication request data transfer object.
    /// </param>
    /// <param name="isOAuth">
    /// A boolean value that indicates if the user is authenticated with OAuth.
    /// </param>
    /// <returns>
    /// The user authentication response data transfer object.
    /// </returns>
    public Task<UserAuthenticationResponseDto?> Authenticate(UserAuthenticationRequestDto userAuthenticationRequestDto, bool isOAuth = false);
    
    /// <summary>
    /// Gets a user by username.
    /// </summary>
    /// <returns>
    /// The user entity.
    /// </returns>
    public Task<User?> GetUserByEmail(string email);

    /// <summary>
    /// Gets a user by username.
    /// </summary>
    /// <returns>
    /// The user entity.
    /// </returns>
    public Task<bool> IsUserRegisteredWithOAuth(string email);
}