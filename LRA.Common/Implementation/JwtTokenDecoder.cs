using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using LRA.Common.Interfaces;
using LRA.Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LRA.Common.Implementation;

public class JwtTokenDecoder : IJwtTokenDecoder
{
    private readonly ILogger<JwtTokenDecoder> _logger;
    private readonly JsonWebKeysList _jwks;

    public JwtTokenDecoder(ILogger<JwtTokenDecoder> logger, IOptions<JsonWebKeysList> jwksOptions)
    {
        _logger = logger;
        _jwks = jwksOptions.Value;
    }
    
public async Task<JwtDecodeResult> DecodeTokenAsync(string token, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Token decoding attempt with empty token.");
            return new JwtDecodeResult { Principal = null };
        }

        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(token))
        {
            _logger.LogWarning("Invalid JWT format for token.");
            return new JwtDecodeResult { Principal = null };
        }

        try
        {
            var jwtToken = handler.ReadJwtToken(token);
            var kid = jwtToken.Header.Kid;
            var jsonWebKey = _jwks.Keys.FirstOrDefault(k => k.KeyId == kid);
            if (jsonWebKey == null)
            {
                _logger.LogWarning("No matching JWK found for kid: {Kid}", kid);
                return new JwtDecodeResult { Principal = null };
            }
            
            var rsaParameters = new RSAParameters
            {
                Modulus = Base64UrlEncoder.DecodeBytes(jsonWebKey.Modulus),
                Exponent = Base64UrlEncoder.DecodeBytes(jsonWebKey.Exponent)
            };
            var rsa = RSA.Create();
            rsa.ImportParameters(rsaParameters);
            
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                NameClaimType = "sub",
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new RsaSecurityKey(rsa),
                SaveSigninToken = true,
            };

            var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);
            var claims = principal.Claims.ToList();

            var subject = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            var clientId = jwtToken.Claims.FirstOrDefault(c => c.Type == "client_id")?.Value;
            var mail = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            if (string.IsNullOrEmpty(subject))
            {
                _logger.LogWarning("Token does not contain 'sub' claim.");
            }

            _logger.LogInformation("Token decoded and validated successfully. Subject: {Subject}, ClientId: {ClientId}.",
                subject ?? "unknown", clientId ?? "unknown");

            return new JwtDecodeResult
            {
                Principal = principal,
                UserId = subject,
                ClientId = clientId,
                Mail = mail
            };
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogError(ex, "Token validation failed.");
            return new JwtDecodeResult { Principal = null };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to decode token.");
            return new JwtDecodeResult { Principal = null };
        }
    }
}
