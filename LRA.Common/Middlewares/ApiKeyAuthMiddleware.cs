using System.Security.Claims;
using LRA.Common.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;

namespace LRA.Common.Middlewares;

public class ApiKeyAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ApiKeysList _apiKeys;

    public ApiKeyAuthMiddleware(RequestDelegate next, IOptions<ApiKeysList> apiKeysConfig)
    {
        _next = next;
        _apiKeys = apiKeysConfig.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("X-Api-Key", out var extractedKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key missing");
            return;
        }

        if (!_apiKeys.ApiKeys.Contains(extractedKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid API Key");
            return;
        }
        
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, extractedKey),
        };

        var identity = new ClaimsIdentity(claims, "ApiKey");
        var principal = new ClaimsPrincipal(identity);

        context.User = principal;

        await _next(context);
    }
}
