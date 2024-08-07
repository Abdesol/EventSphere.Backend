using Microsoft.AspNetCore.Cors.Infrastructure;

namespace EventSphere.Api.Configuration;

/// <summary>
/// Configuration for CORS
/// </summary>
public static class CorsConfig
{
    /// <summary>
    /// Name of the CORS policy
    /// </summary>
    public const string CorsPolicyName = "AllowAll";

    /// <summary>
    /// Configure the CORS policy
    /// </summary>
    /// <param name="options">
    /// The CORS options to configure
    /// </param>
    public static void CorsPolicyConfig(CorsOptions options)
    {
        options.AddPolicy(CorsPolicyName, builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    }
}