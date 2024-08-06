using EventSphere.Application.Services;
using EventSphere.Application.Services.Interfaces;

namespace EventSphere.Api.Middlewares;

public class TokenBlacklistMiddleware(ITokenBlacklistService blacklistService) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var token = authHeader.ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(token) && blacklistService.IsTokenBlacklisted(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }
        }

        await next(context);
    }
}