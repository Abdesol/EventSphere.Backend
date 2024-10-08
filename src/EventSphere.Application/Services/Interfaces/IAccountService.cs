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
    /// Returns the token as a string by a provided user id
    /// </summary>
    /// <param name="id">id of the user</param>
    /// <returns>The JWT token for authentication</returns>
    public Task<string?> GenerateTokenByUserId(int id);
    
    /// <summary>
    /// Gets a user by email.
    /// </summary>
    /// <returns>
    /// The user entity.
    /// </returns>
    public Task<User?> GetUserByEmail(string email);
    
    /// <summary>
    /// Gets a user by id.
    /// </summary>
    /// <returns>
    /// The user entity.
    /// </returns>
    public Task<User?> GetUserById(int id);

    /// <summary>
    /// Gets a user by username.
    /// </summary>
    /// <returns>
    /// The user entity.
    /// </returns>
    public Task<bool> IsUserRegisteredWithOAuth(string email);

    /// <summary>
    /// A method to check if a user exists by id
    /// </summary>
    /// <returns>true if it exists, otherwise, false</returns>
    public Task<bool> DoesUserExist(int id);
    
    /// <summary>
    /// A method to check if a user is already an event organizer or not
    /// </summary>
    public Task<bool> IsUserAlreadyAnEventOrganizer(int id);

    /// <summary>
    /// A method to promote a user from normal user to event organizer
    /// </summary>
    /// <returns>true if successful otherwise false</returns>
    public Task<bool> PromoteToEventOrganizer(int id);

    /// <summary>
    /// A method to set the profile picture of a user
    /// </summary>
    /// <param name="userId">id of the user in the db</param>
    /// <param name="id">id of the picture in the db</param>
    /// <returns>true if successfully done</returns>
    public Task<bool> SetProfilePicture(int userId, string id);
}