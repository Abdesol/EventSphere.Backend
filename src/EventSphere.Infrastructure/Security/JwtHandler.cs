using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EventSphere.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace EventSphere.Infrastructure.Security;

/// <summary>
/// A class that handles the creation of JWT tokens injected to the application services
/// </summary>
/// <param name="configuration"></param>
public class JwtHandler(IConfiguration configuration)
{
    /// <summary>
    /// A configuration section that holds the JWT settings
    /// </summary>
    private readonly IConfigurationSection _jwtSettings = configuration.GetSection("JwtSettings");

    /// <summary>
    /// A method that creates a JWT token
    /// </summary>
    /// <param name="user">
    /// A user object that holds the user's information for the claims
    /// </param>
    /// <returns>
    /// The plain JWT token
    /// </returns>
    public virtual string CreateToken(User user)
    {
        var signingCredentials = GetSigningCredentials();
        var claims = GetClaims(user);
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
        
        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }
    
    /// <summary>
    /// A method that returns the signing credentials
    /// </summary>
    /// <returns>
    /// The signing credentials used to sign the JWT token
    /// </returns>
    private SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(configuration["JWT_SECRET_KEY"]!);
        var secret = new SymmetricSecurityKey(key);

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }
    
    /// <summary>
    /// A method that returns the claims used to create the JWT token
    /// </summary>
    /// <param name="user">
    /// A user object that holds the user's information for the claims
    /// </param>
    /// <returns>
    /// A list of claims used to create the JWT token
    /// </returns>
    private static List<Claim> GetClaims(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Role, user.Role!)
        };

        return claims;
    }


    /// <summary>
    /// A method that extracts the user ID from a JWT token.
    /// </summary>
    /// <param name="authorization">
    /// The authorization header that holds the JWT token.
    /// </param>
    /// <returns>
    /// The user ID as a string.
    /// </returns>
    public virtual string? GetUserEmail(StringValues authorization)
    {        
        var token = authorization.ToString().Split(" ")[1];

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        var userEmailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
        
        return userEmailClaim?.Value;
    }
    
    /// <summary>
    /// A method that generates the token options for the JWT token
    /// </summary>
    /// <param name="signingCredentials">
    /// The signing credentials used to sign the JWT token
    /// </param>
    /// <param name="claims">
    /// The claims used to create the JWT token
    /// </param>
    /// <returns>
    /// A JWT security token that holds the token.
    /// </returns>
    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var tokenOptions = new JwtSecurityToken(
            issuer: _jwtSettings["Issuer"],
            audience: _jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSettings["ExpiryInMinutes"])),
            signingCredentials: signingCredentials
        );

        return tokenOptions;
    }
}