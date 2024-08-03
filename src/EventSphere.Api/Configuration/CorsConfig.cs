using Microsoft.AspNetCore.Cors.Infrastructure;

namespace EventSphere.Api.Configuration;

public static class CorsConfig
{
    public const string CorsPolicyName = "AllowAll";

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