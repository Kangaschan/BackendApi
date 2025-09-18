using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using LRA.Common.DTOs;
using LRA.Common.DTOs.Auth;
using LRA.Common.DTOs.KYC;
using LRA.Common.Exceptions;
using LRA.Common.Models;
using LRA.Gateways.Client.Configuration;
using LRA.Gateways.Client.DTOs;
using LRA.Gateways.Client.Interfaces;
using Microsoft.Extensions.Options;

namespace LRA.Gateways.Client.Implementations;

public class AccountServiceClient : IAccountServiceClient
{
    private readonly AccountHttpClientConfig _accountHttpClientConfig;
    private readonly HttpClient _httpClient;

    public AccountServiceClient(
        IOptions<AccountHttpClientConfig> accountRouteConfig,
        HttpClient httpClient)
    {
        _accountHttpClientConfig = accountRouteConfig.Value;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(_accountHttpClientConfig.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", _accountHttpClientConfig.ApiKey);
    }
    
    public async Task RegisterAsync(RegisterAccountRequestDto accountRequest, CancellationToken cancellationToken)
    {
        var requestPath = $"{_accountHttpClientConfig.Route}register";
        var request = new RegisterAccountDto
        {
            Email = accountRequest.Email,
            Password = accountRequest.Password,
        };
        using (var response = await _httpClient.PostAsJsonAsync(requestPath, request, cancellationToken))
        {
            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                throw new UserAlreadyExistsException($"User with email {accountRequest.Email} already exists");
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Registration error. Code: {response.StatusCode}. service answer: {errorContent}");
            }
        }
    }

    public async Task VerifyAsync(TokenDto token, CancellationToken cancellationToken)
    {
        var requestPath = $"{_accountHttpClientConfig.Route}email/verifications";
        
        using (var response = await _httpClient.PostAsJsonAsync(requestPath, token, cancellationToken))
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException("verification token not found");
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new TokenOutdatedException("verification token expired");
            }
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Verification error. Code: {response.StatusCode}. service answer: {errorContent}");
            }
        }
    }

    public async Task<JwtTokenDto> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        var requestPath = $"{_accountHttpClientConfig.Route}auth/login";
        
        using (var response = await _httpClient.PostAsJsonAsync(requestPath, loginRequest, cancellationToken))
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new Required2FaException("2fa authentication needed");
            }

            if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.Unauthorized)
            {
                throw new HttpRequestException($"Login failed with status code: {response.StatusCode}");
            }
            var token = await response.Content.ReadFromJsonAsync<JwtTokenDto>(cancellationToken);

            return token;
        }
    }
    
    public async Task<JwtTokenDto> RefreshAsync(RefreshTokenDto refreshToken, CancellationToken cancellationToken)
    {
        var requestPath = $"{_accountHttpClientConfig.Route}auth/refresh";
        
        using (var response = await _httpClient.PostAsJsonAsync(requestPath, refreshToken, cancellationToken))
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Refresh error. Code: {response.StatusCode}. service answer: {errorContent}");
            }
            var token = await response.Content.ReadFromJsonAsync<JwtTokenDto>(cancellationToken);
            return token;
        }
    }

    public async Task<JsonWebKeysList> GetJsonWebKeyAsync(CancellationToken cancellationToken)
    {
        var requestPath = $"{_accountHttpClientConfig.Route}jwks";
        using (var response = await _httpClient.GetAsync(requestPath, cancellationToken))
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Obtaining jwt keys error. Code: {response.StatusCode}. service answer: {errorContent}");
            }
            var jsonWebKeysList = await response.Content.ReadFromJsonAsync<JsonWebKeysList>(cancellationToken);
            return jsonWebKeysList;
        }
    }
    
    public async Task<(JwtTokenDto, bool)> Complete2FaAsync(Complete2FaRequest сomplete2FaRequest, CancellationToken cancellationToken)
    {
        var requestPath = $"{_accountHttpClientConfig.Route}auth/2fa";
        
        using (var response = await _httpClient.PostAsJsonAsync(requestPath, сomplete2FaRequest, cancellationToken))
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return (null, false);
            }
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"2fa error. Code: {response.StatusCode}. service answer: {errorContent}");
            }
            else
            {
                var res = await response.Content.ReadFromJsonAsync<JwtTokenDto>(cancellationToken);
                return (res, true);
            }
            
        }
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest, CancellationToken cancellationToken)
    {
        var requestPath = $"{_accountHttpClientConfig.Route}password/recovery";
        
        using (var response = await _httpClient.PostAsJsonAsync(requestPath, resetPasswordRequest, cancellationToken))
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Password recovery error. Code: {response.StatusCode}. service answer: {errorContent}");
            }
        }
    }

    public async Task CompletePasswordResetAsync(CompleteResetPasswordRequest completeResetPasswordRequest, CancellationToken cancellationToken)
    {
        var requestPath = $"{_accountHttpClientConfig.Route}password/recovery";

        using (var response =
               await _httpClient.PutAsJsonAsync(requestPath, completeResetPasswordRequest, cancellationToken))
        {
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new TokenOutdatedException("one time password expired");
            }
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Password changing error. Code: {response.StatusCode}. service answer: {errorContent}");
            }
        }
    }

    public async Task ChangePasswordAsync(string mail, ChangePasswordRequest changePasswordRequest, CancellationToken cancellationToken)
    {
        var requestPath = $"{_accountHttpClientConfig.Route}password/{mail}";

        using (var response = await _httpClient.PutAsJsonAsync(requestPath, changePasswordRequest, cancellationToken))
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Password change error. Code: {response.StatusCode}. service answer: {errorContent}");
            }
        }
    }
    
    public async Task ChangeMailAsync(string keycloakId, ChangeEmailRequest changeEmailRequest, CancellationToken cancellationToken)
    {
        var requestPath = $"{_accountHttpClientConfig.Route}email/change-request/{keycloakId}";

        using (var response = await _httpClient.PostAsJsonAsync(requestPath, changeEmailRequest, cancellationToken))
        {
            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                throw new UserAlreadyExistsException("email already exists");
            }
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Password change error. Code: {response.StatusCode}. service answer: {errorContent}");
            }
        }
    }

    public async Task<bool> CompleteChangeMailAsync(TokenDto token, CancellationToken cancellationToken)
    {
        var requestPath = $"{_accountHttpClientConfig.Route}email";
        
        using (var response = await _httpClient.PutAsJsonAsync(requestPath, token, cancellationToken))
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public async Task ChangeFullNameAsync(string keycloakId, ChangeFullNameRequest changeFullNameRequest,
        CancellationToken cancellationToken)
    {
        var requestPath = $"{_accountHttpClientConfig.Route}profile/fullname/{keycloakId}";

        using (var response = await _httpClient.PutAsJsonAsync(requestPath, changeFullNameRequest, cancellationToken))
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Fullname change error. Code: {response.StatusCode}. service answer: {errorContent}");
            }
        }
    }

    public async Task Change2FaAsync(string keycloakId, Change2FaRequest change2FaRequest, CancellationToken cancellationToken)
    {
        var requestPath = $"{_accountHttpClientConfig.Route}profile/2fa/{keycloakId}";

        using (var response = await _httpClient.PutAsJsonAsync(requestPath, change2FaRequest, cancellationToken))
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"2fa change error. Code: {response.StatusCode}. service answer: {errorContent}");
            }
        }
    }

    public async Task<RedirectUrl> GoogleLoginAsync(CancellationToken cancellationToken)
    {
        var requestPath = $"{_accountHttpClientConfig.Route}auth/google";
        
        using (var response = await _httpClient.GetAsync(requestPath, cancellationToken))
        {
            var redirectUrl = await response.Content.ReadFromJsonAsync<RedirectUrl>(cancellationToken);
            return redirectUrl;
        }
    }

    public async Task<JwtTokenDto> CompleteGoogleLoginAsync(string code, CancellationToken cancellationToken)
    {
        var requestPath = $"{_accountHttpClientConfig.Route}auth/google/callback?code={Uri.EscapeDataString(code)}";
        
        using (var response = await _httpClient.GetAsync(requestPath, cancellationToken))
        {
            var token = await response.Content.ReadFromJsonAsync<JwtTokenDto>(cancellationToken);
            return token;
        }
    }

    public async Task ApplyForAJobAsync(string email, JobApplicationDto jobApplication, CancellationToken cancellationToken)
    {
        var requestPath = $"{_accountHttpClientConfig.Route}profile/job-applications/{email}";
    
        using var formData = new MultipartFormDataContent();
    
        await AddFileToFormData(formData, jobApplication.IdentityDocumentPhoto, "IdentityDocumentPhoto");
        await AddFileToFormData(formData, jobApplication.IdentityDocumentSelfie, "IdentityDocumentSelfie");
        await AddFileToFormData(formData, jobApplication.MedicalCertificatePhoto, "MedicalCertificatePhoto");
    
        using var response = await _httpClient.PostAsync(requestPath, formData, cancellationToken);
    
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            throw new ArgumentException("You've already added a job application");
        }
    
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Job application error. Code: {response.StatusCode}. Service answer: {errorContent}");
        }
    }

    private async Task AddFileToFormData(MultipartFormDataContent formData, IFormFile file, string name)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException($"File {name} is required");
    
        var fileContent = new StreamContent(file.OpenReadStream());
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
        formData.Add(fileContent, name, file.FileName);
    }
    
    public async Task<IEnumerable<KycListItemDto>?> CheckKycStatusAsync(string email, CancellationToken cancellationToken)
    {
        var requestPath = $"{_accountHttpClientConfig.Route}profile/job-applications/{email}";
        
        using (var response = await _httpClient.GetAsync(requestPath, cancellationToken))
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"job-applications error. Code: {response.StatusCode}. service answer: {errorContent}");
            }
            var applicationList = await response.Content.ReadFromJsonAsync<IEnumerable<KycListItemDto>>(cancellationToken);
            return applicationList;
            
        }
    }
}
