using System.Text.Encodings.Web;
using LRA.Common.Interfaces;
using LRA.Gateways.Admin.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace LRA.Gateways.Admin.Implementations;

public class JwtAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ILogger<JwtAuthenticationHandler> _logger;
    private readonly IJwtTokenDecoder _jwtTokenDecoder;
    private readonly IAccountServiceClient _accountServiceClient;

    public JwtAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory loggerFactory,
        UrlEncoder encoder,
        ISystemClock clock,
        IJwtTokenDecoder jwtTokenDecoder,
        IAccountServiceClient accountServiceClient
    )
        : base(options, loggerFactory, encoder, clock)
    {
        _logger = loggerFactory.CreateLogger<JwtAuthenticationHandler>();
        _jwtTokenDecoder = jwtTokenDecoder;
        _accountServiceClient = accountServiceClient;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            _logger.LogWarning("Authorization header is missing.");
            return AuthenticateResult.NoResult();
        }

        var authHeader = Request.Headers["Authorization"].ToString();
        if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Invalid Authorization header format. Expected 'Bearer <token>'.");
            return AuthenticateResult.Fail("Invalid Authorization header format");
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Bearer token is empty.");
            return AuthenticateResult.Fail("Bearer token is empty");
        }

        try
        {
            var decodeResult = await _jwtTokenDecoder.DecodeTokenAsync(token, Context.RequestAborted);
            if (decodeResult.Principal == null)
            {
                _logger.LogWarning("Failed to decode token.");
                return AuthenticateResult.Fail("Failed to decode token");
            }

            Context.Items["UserId"] = decodeResult.UserId;
            Context.Items["ClientId"] = decodeResult.ClientId;
            Context.Items["Mail"] = decodeResult.Mail;  
            var isAdmin = await _accountServiceClient.CheckAdminRolesByEmail(decodeResult.Mail, Context.RequestAborted);
            if (!isAdmin.IsAdmin && !isAdmin.IsSuperAdmin)
            {
                _logger.LogWarning("User {UserId} is not an admin.",decodeResult.UserId ?? "unknown" );
                return AuthenticateResult.Fail("User is not an admin");
            }
            Context.Items["IsAdmin"] = isAdmin.IsAdmin;
            Context.Items["IsSuperAdmin"] = isAdmin.IsSuperAdmin;
            
            var ticket = new AuthenticationTicket(decodeResult.Principal, Scheme.Name);
            _logger.LogInformation("Token processed successfully for user {UserId}, client_id: {ClientId}.",
                decodeResult.UserId ?? "unknown",
                decodeResult.ClientId ?? "unknown");

            return AuthenticateResult.Success(ticket);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token processing failed.");
            return AuthenticateResult.Fail($"Token processing failed: {ex.Message}");
        }
    }
}
