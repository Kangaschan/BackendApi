using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using LRA.Account.Application.DTOs;
using LRA.Account.Application.Interfaces;
using LRA.Account.KeycloakInfrastructure.Configuration;
using LRA.Account.KeycloakInfrastructure.Models;
using LRA.Common.DTOs.Auth;
using LRA.Common.Exceptions;
using LRA.Common.Models;
using Microsoft.Extensions.Options;

namespace LRA.Account.KeycloakInfrastructure.Implementations;

public class KeycloakServiceClient : IKeycloakServiceClient
{
    private readonly KeycloakHttpClientConfig _keycloakHttpClientConfig;
    private readonly HttpClient _httpClient;

    public KeycloakServiceClient(IOptions<KeycloakHttpClientConfig> keycloakHttpClientConfig, HttpClient httpClient)
    {
        _keycloakHttpClientConfig = keycloakHttpClientConfig.Value;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(_keycloakHttpClientConfig.BaseUrl);
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    private async Task<string?> GetAdminTokenAsync(CancellationToken cancellationToken)
    {
        var tokenRequest = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", "admin-cli" },
            { "client_secret", _keycloakHttpClientConfig.AdminClientSecret }
        };

        using (var response = await _httpClient.PostAsync(
            $"{_keycloakHttpClientConfig.AdminTokenRoute}",
            new FormUrlEncodedContent(tokenRequest),
            cancellationToken))
        {
            if (!response.IsSuccessStatusCode)
            {   
                var errorContent = await response.Content.ReadFromJsonAsync<string>(cancellationToken);
                throw new HttpRequestException($"Failed to get admin token: {response.StatusCode}, {errorContent}");
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>(cancellationToken);
            return tokenResponse.AccessToken;
        }
    }

    public async Task<KeycloakId> CreateAccountAsync(KeycloakAccountCreateRequest accountCreateRequest,
        CancellationToken cancellationToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", await GetAdminTokenAsync(cancellationToken));

        using (var response = await _httpClient.PostAsync(
            _keycloakHttpClientConfig.AdminRoute,
            new StringContent(JsonSerializer.Serialize(accountCreateRequest), Encoding.UTF8, "application/json"),
            cancellationToken))
        {
            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                throw new UserAlreadyExistsException($"User with email {accountCreateRequest.Email} already exists");
            }
            var location = response.Headers.Location?.ToString();
            var keycloakId = location?.Split('/').Last();
            return new KeycloakId
            {
                Key = keycloakId
            };
        }
    }

    public async Task UpdateAccountAsync(KeycloakId id, KeycloakAccountUpdateRequest account,
        CancellationToken cancellationToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", await GetAdminTokenAsync(cancellationToken));

        using var response = await _httpClient.PutAsync(
            $"{_keycloakHttpClientConfig.AdminRoute}/{id.Key}",
            new StringContent(JsonSerializer.Serialize(account), Encoding.UTF8, "application/json"),
            cancellationToken);
    }

    public async Task ActivateAccountAsync(KeycloakId id, CancellationToken cancellationToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", await GetAdminTokenAsync(cancellationToken));
        var activateDto = new KeycloakAccountEnableteDto
        {
            Enabled = true
        };
        using var response = await _httpClient.PutAsync(
            $"{_keycloakHttpClientConfig.AdminRoute}/{id.Key}",
            new StringContent(JsonSerializer.Serialize(activateDto), Encoding.UTF8, "application/json"),
            cancellationToken);
    }

    public async Task<KeycloakTokenResponse> LoginAccountAsync(LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        var tokenUrl = _keycloakHttpClientConfig.BaseUrl + _keycloakHttpClientConfig.LoginRoute;

        var requestData = new Dictionary<string, string>
        {
            { "grant_type", "password" },
            { "client_id", "lra-auth" },
            { "client_secret", _keycloakHttpClientConfig.LoginClientSecret },
            { "username", loginRequest.Email },
            { "password", loginRequest.Password }
        };
        var content = new FormUrlEncodedContent(requestData);

        using (var response = await _httpClient.PostAsync(tokenUrl, content, cancellationToken))
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadFromJsonAsync<KeycloakErrorResponse>(cancellationToken);
                throw new InvalidOperationException($"Login failed: {response.StatusCode}, {errorContent}");
            }
            var tokenResponse = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>(cancellationToken);
            if (tokenResponse.AccessToken == null)
            {
                throw new Exception("Login failed");
            }

            return tokenResponse;
        }
    }

    public async Task<KeycloakTokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var tokenUrl = _keycloakHttpClientConfig.BaseUrl + _keycloakHttpClientConfig.LoginRoute;

        var requestData = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "client_id", "lra-auth" },
            { "client_secret", _keycloakHttpClientConfig.LoginClientSecret },
            { "refresh_token", refreshToken }
        };

        var content = new FormUrlEncodedContent(requestData);

        using (var response = await _httpClient.PostAsync(tokenUrl, content, cancellationToken))
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadFromJsonAsync<string>(cancellationToken);
                throw new InvalidOperationException($"Token refresh failed: {response.StatusCode}, {errorContent}");
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>(cancellationToken);

            return tokenResponse;
        }
    }

    public async Task DeleteAccountAsync(KeycloakId id, CancellationToken cancellationToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", await GetAdminTokenAsync(cancellationToken));

        using var response = await _httpClient.DeleteAsync(
            $"{_keycloakHttpClientConfig.AdminRoute}/{id.Key}",
            cancellationToken);
    }

    public async Task ChangePasswordAsync(ChangePasswordRequest changePasswordRequest, KeycloakId keycloakId, CancellationToken cancellationToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Bearer", await GetAdminTokenAsync(cancellationToken));

        var resetPasswordRequest = new
        {
            type = "password",
            value = changePasswordRequest.NewPassword,
            temporary = false
        };

        using (var resetResponse = await _httpClient.PutAsync(
            $"{_keycloakHttpClientConfig.AdminRoute}/{keycloakId.Key}/reset-password",
            new StringContent(JsonSerializer.Serialize(resetPasswordRequest), Encoding.UTF8, "application/json"),
            cancellationToken))
        {
            if (!resetResponse.IsSuccessStatusCode)
            {
                var errorContent = await resetResponse.Content.ReadFromJsonAsync<string>(cancellationToken);
                throw new InvalidOperationException(
                    $"Password change failed: {resetResponse.StatusCode}, {errorContent}");
            }
        }
    }

    public async Task<JsonWebKeysList> GetJwksAsync(CancellationToken cancellationToken)
    {
        var jwksUrl = _keycloakHttpClientConfig.BaseUrl + _keycloakHttpClientConfig.CertsRoute;
        using (var response = await _httpClient.GetAsync(jwksUrl, cancellationToken))
        {
            var jwks = await response.Content.ReadFromJsonAsync<JsonWebKeysList>(cancellationToken);
            return jwks;
        }
    }

    public async Task ResetPasswordAsync(string newPassword, KeycloakId keycloakId, CancellationToken cancellationToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Bearer", await GetAdminTokenAsync(cancellationToken));

        var resetPasswordRequest = new
        {
            type = "password",
            value = newPassword,
            temporary = false
        };

        using (var resetResponse = await _httpClient.PutAsync(
            $"{_keycloakHttpClientConfig.AdminRoute}/{keycloakId.Key}/reset-password",
            new StringContent(JsonSerializer.Serialize(resetPasswordRequest), Encoding.UTF8, "application/json"),
            cancellationToken))
        {
            if (!resetResponse.IsSuccessStatusCode)
            {
                var errorContent = await resetResponse.Content.ReadFromJsonAsync<string>(cancellationToken);
                throw new InvalidOperationException(
                    $"Password change failed: {resetResponse.StatusCode}, {errorContent}");
            }
        }
    }

    public async Task CompleteEmailChangeAsync(KeycloakId keycloakId, string newEmail,
        CancellationToken cancellationToken)
    {
        var changeEmailRequest = new CompleteChangeEmailRequest
        {
            Email = newEmail,
            EmailVerified = true
        };
        
        using (var response = await _httpClient.PutAsync(
            $"{_keycloakHttpClientConfig.AdminRoute}/{keycloakId.Key}",
            new StringContent(JsonSerializer.Serialize(changeEmailRequest), Encoding.UTF8, "application/json"),
            cancellationToken))
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadFromJsonAsync<string>(cancellationToken);
                throw new InvalidOperationException($"Email change failed: {response.StatusCode}, {errorContent}");
            }
        }
    }

    public string GetGoogleAuthUrl()
    {
        var authUrl = _keycloakHttpClientConfig.GoogleLoginRoute;
        var query = new Dictionary<string, string>
        {
            { "client_id", "lra-auth" },
            { "response_type", "code" },
            { "redirect_uri", _keycloakHttpClientConfig.CallbackRoute },
            { "scope", "openid profile email" },
            { "kc_idp_hint", "google" }
        };
        var queryString = string.Join("&", query.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
        return $"{authUrl}?{queryString}";
    }
    
    public async Task<KeycloakTokenResponse> ExchangeCodeForTokenAsync(string code, CancellationToken cancellationToken)
    {
        var tokenUrl = $"{_keycloakHttpClientConfig.BaseUrl}/realms/lra-realm/protocol/openid-connect/token";
        var requestData = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "client_id", "lra-auth" },
            { "client_secret", _keycloakHttpClientConfig.LoginClientSecret },
            { "code", code },
            { "redirect_uri", _keycloakHttpClientConfig.CallbackRoute }
        };
        var content = new FormUrlEncodedContent(requestData);
        
        using (var response = await _httpClient.PostAsync(tokenUrl, content, cancellationToken))
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadFromJsonAsync<string>(cancellationToken);
                throw new InvalidOperationException($"Token exchange failed: {response.StatusCode}, {errorContent}");
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>(cancellationToken);
            return tokenResponse;
        }
    }
    
    public async Task<string> GetUserEmailByIdAsync(KeycloakId keycloakId, CancellationToken cancellationToken)
    {
        var userUrl = $"{_keycloakHttpClientConfig.BaseUrl}{_keycloakHttpClientConfig.AdminRoute}/{keycloakId.Key}";
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetAdminTokenAsync(cancellationToken));
       
        using (var response = await _httpClient.GetAsync(userUrl, cancellationToken))
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadFromJsonAsync<string>(cancellationToken);
                throw new InvalidOperationException($"Failed to fetch user: {response.StatusCode}, {errorContent}");
            }

            var user = await response.Content.ReadFromJsonAsync<KeycloakUserRepresentation>(cancellationToken);
            return user.Email;
        }
    }
    
    public async Task<(KeycloakId, string)> GetUserInfoFromAccessTokenAsync(string accessToken, CancellationToken cancellationToken)
    {
        var introspectUrl = $"{_keycloakHttpClientConfig.BaseUrl}{_keycloakHttpClientConfig.LoginRoute}/introspect";
        
        var requestData = new Dictionary<string, string>
        {
            { "token", accessToken },
            { "client_id", "lra-auth" },
            { "client_secret", _keycloakHttpClientConfig.LoginClientSecret }
        };

        var content = new FormUrlEncodedContent(requestData);
        
        using (var response = await _httpClient.PostAsync(introspectUrl, content, cancellationToken))
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadFromJsonAsync<string>(cancellationToken);
                throw new InvalidOperationException(
                    $"Token introspection failed: {response.StatusCode}, {errorContent}");
            }

            var introspectionResponse = await response.Content.ReadFromJsonAsync<KeycloakIntrospectionResponse>(cancellationToken);

            if (!introspectionResponse.Active)
            {
                throw new InvalidOperationException("Provided access token is not active");
            }

            return (new KeycloakId { Key = introspectionResponse.Sub }, introspectionResponse.Username);
        }
    }
}
