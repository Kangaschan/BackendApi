using System.Data;
using System.Net;
using LRA.Common.DTOs;
using LRA.Common.DTOs.Auth;
using LRA.Common.DTOs.KYC;
using LRA.Common.Exceptions;
using LRA.Common.Models;
using LRA.Gateways.Admin.Configurations;
using LRA.Gateways.Admin.DTOs;
using LRA.Gateways.Admin.Interfaces;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using LoginRequest = LRA.Common.DTOs.Auth.LoginRequest;
using static LRA.Common.Utils.PasswordGenerationUtils;
using ResetPasswordRequest = LRA.Gateways.Admin.DTOs.ResetPasswordRequest;

namespace LRA.Gateways.Admin.Implementations;

public class AccountServiceClient : IAccountServiceClient
{
    private readonly AccountHttpClientConfig _accountHttpClientConfig;
    private readonly HttpClient _httpClient;
    private readonly ICurrentUserContext _userContext;

    public AccountServiceClient(
        IOptions<AccountHttpClientConfig> accountRouteConfig,
        HttpClient httpClient,
        ICurrentUserContext userContext)
    {
        _accountHttpClientConfig = accountRouteConfig.Value;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(_accountHttpClientConfig.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", _accountHttpClientConfig.ApiKey);
        _userContext = userContext;
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

    public async Task<AccountRolesResponse> CheckAdminRolesByEmail(string email, CancellationToken cancellationToken)
    {
        var requestPath = $"{_accountHttpClientConfig.Route}account/roles/by-email/{email}";

        using (var response = await _httpClient.GetAsync(requestPath, cancellationToken))
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException($"No account found for email: {email}");
            }
            
            var roles = await response.Content.ReadFromJsonAsync<AccountRoles>(cancellationToken);
            var accountRolesResponse = new AccountRolesResponse
            {
                IsAdmin = false,
                IsSuperAdmin = false
            };
            if (roles.Roles.Contains("admin", StringComparer.OrdinalIgnoreCase))
            {
                accountRolesResponse.IsAdmin = true;
            }
            else
            {
                accountRolesResponse.IsAdmin = false;
            }

            if (roles.Roles.Contains("superAdmin", StringComparer.OrdinalIgnoreCase))
            {
                accountRolesResponse.IsSuperAdmin = true;
            }
            else
            {
                accountRolesResponse.IsSuperAdmin = false;
            }
            
            return accountRolesResponse;
        }
    }

    public async Task<AccountRolesResponse> CheckAdminRolesById(Guid id,CancellationToken cancellationToken)
    {
        var requestPath = $"{_accountHttpClientConfig.Route}account/roles/by-id/{id}";

        using (var response = await _httpClient.GetAsync(requestPath, cancellationToken))
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException($"No account found for id: {id}");
            }
            
            var roles = await response.Content.ReadFromJsonAsync<AccountRoles>(cancellationToken);
            var accountRolesResponse = new AccountRolesResponse
            {
                IsAdmin = false,
                IsSuperAdmin = false,
                Email = roles.Email
            };
            if (roles.Roles.Contains("admin", StringComparer.OrdinalIgnoreCase))
            {
                accountRolesResponse.IsAdmin = true;
            }
            else
            {
                accountRolesResponse.IsAdmin = false;
            }

            if (roles.Roles.Contains("superAdmin", StringComparer.OrdinalIgnoreCase))
            {
                accountRolesResponse.IsSuperAdmin = true;
            }
            else
            {
                accountRolesResponse.IsSuperAdmin = false;
            }
            
            return accountRolesResponse;
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
            if (response.StatusCode == HttpStatusCode.Gone)
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

    public async Task ChangePasswordAsync(ChangePasswordRequest changePasswordRequest, CancellationToken cancellationToken)
    {
        var requestPath = $"{_accountHttpClientConfig.Route}password";

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

    public async Task<CreateAccountRequestDto> CreateAccountAsync(AdminCreateAccountRequest request, CancellationToken cancellationToken)
    {
        var requestPath = $"{_accountHttpClientConfig.Route}";
        if (request.Roles.Contains("admin", StringComparer.OrdinalIgnoreCase) ||
            request.Roles.Contains("superAdmin", StringComparer.OrdinalIgnoreCase))
        {
            if (!_userContext.IsSuperAdmin)
            {
                throw new UnauthorizedAccessException("You do not have super admin rights");
            }
        }
        var createAccountDto = new CreateAccountRequestDto
        {
            Email = request.Email,
            FirstName = "",
            LastName = "",
            IsTemporaryPassword = true,
            Password = GenerateSecurePassword(),
            Phone = "",
            Roles = request.Roles
        };
        
        using (var response = await _httpClient.PostAsJsonAsync(requestPath, createAccountDto, cancellationToken))
        {
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidDataException(errorContent);
            }
        }
        return createAccountDto;
    }
    
    public async Task<PagedResult<AccountSearchResult>?> GetFilteredAsync(ViewFilterParams filterParams, CancellationToken cancellationToken)
{
    var basePath = $"{_accountHttpClientConfig.Route}filtered";
    var queryParams = new Dictionary<string, string?>
    {
        ["PageSize"] = filterParams.PageSize.ToString(),
        ["PageNumber"] = filterParams.PageNumber.ToString(),
        ["IncludeAdmins"] = filterParams.IncludeAdmins?.ToString(),
        ["IncludeClients"] = filterParams.IncludeClients?.ToString(),
        ["IncludeParamedics"] = filterParams.IncludeParamedics?.ToString(),
        ["IncludeBlocked"] = filterParams.IncludeBlocked?.ToString()
    };
    
    var requestUri = QueryHelpers.AddQueryString(basePath, queryParams.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value!));
    
    using (var response = await _httpClient.GetAsync(requestUri, cancellationToken))
    {
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidDataException(errorContent);
        }
        var accounts = await response.Content.ReadFromJsonAsync<PagedResult<AccountSearchResult>?>(cancellationToken);
        
        return accounts;
    }
}

public async Task<PagedResult<AccountSearchResult>?> SearchAsync(AccountSearchParams searchParams, CancellationToken cancellationToken)
{
    var basePath = $"{_accountHttpClientConfig.Route}by-criteria";
    var queryParams = new Dictionary<string, string?>
    {
        ["PageSize"] = searchParams.PageSize.ToString(),
        ["PageNumber"] = searchParams.PageNumber.ToString(),
        ["IncludeAdmins"] = searchParams.IncludeAdmins?.ToString(),
        ["IncludeClients"] = searchParams.IncludeClients?.ToString(),
        ["IncludeParamedics"] = searchParams.IncludeParamedics?.ToString(),
        ["IncludeBlocked"] = searchParams.IncludeBlocked?.ToString(),
        ["EmailSearch"] = searchParams.EmailSearch,
        ["PhoneSearch"] = searchParams.PhoneSearch,
        ["FirstNameSearch"] = searchParams.FirstNameSearch,
        ["LastNameSearch"] = searchParams.LastNameSearch
    };
    
    var requestUri = QueryHelpers.AddQueryString(basePath, queryParams.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value!));
    
    using (var response = await _httpClient.GetAsync(requestUri, cancellationToken))
    {
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidDataException(errorContent);
        }
        var accounts = await response.Content.ReadFromJsonAsync<PagedResult<AccountSearchResult>?>(cancellationToken);
        
        return accounts;
    }
}

    public async Task<AccountDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var requestPath = $"{_accountHttpClientConfig.Route}{id}";
        using (var response = await _httpClient.GetAsync(requestPath, cancellationToken))
        {
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return null;
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidDataException(errorContent);
            }
            var account = await response.Content.ReadFromJsonAsync<AccountDto?>(cancellationToken);
            return account;
        }
    }

    public async Task UpdateAccountAsync(Guid id, AccountDto updatedAccount, CancellationToken cancellationToken)
    {
        var requestPath = $"{_accountHttpClientConfig.Route}{id}";
        var updateAccountRoles = await CheckAdminRolesByEmail(updatedAccount.Email, cancellationToken);
        if (((updateAccountRoles.IsSuperAdmin || updateAccountRoles.IsAdmin) && !_userContext.IsSuperAdmin) ||
            (updatedAccount.Roles.Contains("admin", StringComparer.OrdinalIgnoreCase) || updatedAccount.Roles.Contains("superAdmin", StringComparer.OrdinalIgnoreCase) && !_userContext.IsSuperAdmin))
        {
            throw new UnauthorizedAccessException("You do not have super admin rights");
        }

        using (var response = await _httpClient.PutAsJsonAsync(requestPath, updatedAccount, cancellationToken))
        {
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidDataException(errorContent);
            }
        }
    }

    public async Task<string> ResetAccountPasswordAsync(string email, ResetOptions resetOptions, CancellationToken cancellationToken)
    {
        var updateAccountRoles = await CheckAdminRolesByEmail(email, cancellationToken);
        if (((updateAccountRoles.IsSuperAdmin || updateAccountRoles.IsAdmin) && !_userContext.IsSuperAdmin))
        {
            throw new UnauthorizedAccessException("You do not have super admin rights");
        }

        if (resetOptions.GenerateNewPassword)
        {
            var adminPasswordChange = new AdminChangePasswordRequest
            {
                NewPassword = GenerateSecurePassword(),
            };
            var requestPath = $"{_accountHttpClientConfig.Route}password/admin-reset/{email}";
            using (var response = await _httpClient.PutAsJsonAsync(requestPath, adminPasswordChange, cancellationToken))
            {
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    throw new InvalidDataException(errorContent);
                }

                return ($"password reset, temp password is {adminPasswordChange.NewPassword}");
            }
        }
        else
        {
            var resetPasswordRequest = new ResetPasswordRequest
            {
                Email = email,
            };
            await ResetPasswordAsync(resetPasswordRequest, cancellationToken);
            return ("message for password reset sent to email");
        }
    }

    public async Task ChangeBlockStatus(Guid id, BlockRequest blockRequest, CancellationToken cancellationToken)
    {
        var accountRoles = await CheckAdminRolesById(id, cancellationToken);
        if (!_userContext.IsSuperAdmin && (accountRoles.IsSuperAdmin || accountRoles.IsAdmin))
        {
            throw new UnauthorizedAccessException("You do not have super admin rights");
        }

        if (accountRoles.Email == _userContext.Email)
        {
            throw new InvalidExpressionException("You cant block yourself");
        }
        
        var requestPath = $"{_accountHttpClientConfig.Route}{id}/block";

        using (var response = await _httpClient.PutAsJsonAsync(requestPath, blockRequest, cancellationToken))
        {
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidDataException(errorContent);
            }
        }
    }
    
    public async Task DeleteAccountAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!_userContext.IsSuperAdmin)
        {
            throw new UnauthorizedAccessException("You do not have super admin rights");
        }
        var requestPath = $"{_accountHttpClientConfig.Route}{id}";
        using (var response = await _httpClient.DeleteAsync(requestPath, cancellationToken))
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"User Delete error. Code: {response.StatusCode}. service answer: {errorContent}");
            }
        }
    }
    
    public async Task SoftDeleteAccountAsync(Guid id, CancellationToken cancellationToken)
    {
        var requestPath = $"{_accountHttpClientConfig.Route}{id}/soft";
        using (var response = await _httpClient.DeleteAsync(requestPath, cancellationToken))
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"User Delete error. Code: {response.StatusCode}. service answer: {errorContent}");
            }
        }
    }
    
    public async Task<IEnumerable<KycListItemDto>?> GetAllJobApplicationsAsync(CancellationToken cancellationToken)
    {
        var requestPath = $"{_accountHttpClientConfig.Route}job-applications";
        using (var response = await _httpClient.GetAsync(requestPath, cancellationToken))
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Get all job applications error. Code: {response.StatusCode}. service answer: {errorContent}");
            }
            var jobApplications = await response.Content.ReadFromJsonAsync<IEnumerable<KycListItemDto>>(cancellationToken);
            return jobApplications;
        }
    }
    
    public async Task<JobApplication?> GetDetailedJobApplicationAsync(Guid id, CancellationToken cancellationToken)
    {
        var requestPath = $"{_accountHttpClientConfig.Route}job-applications/{id}";
        using (var response = await _httpClient.GetAsync(requestPath, cancellationToken))
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Get detail job application error: {response.StatusCode}. service answer: {errorContent}");
            }   
            var jobApplication = await response.Content.ReadFromJsonAsync<JobApplication>(cancellationToken);
            return jobApplication;
        }
    }

    public async Task ProcessJobApplicationAsync(JobApplicationProcessDto jobApplication,
        CancellationToken cancellationToken)
    {
        var requestPath = $"{_accountHttpClientConfig.Route}job-applications";
        var kycProcessDto = new KycProcessDto
        {
            KycId = jobApplication.KycId,
            Status = jobApplication.Status,
            RejectReason = jobApplication.RejectReason,
            AdminEmail = _userContext.Email,
        };
        using (var response = await _httpClient.PutAsJsonAsync(requestPath, kycProcessDto, cancellationToken))
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Process job application error. Code: {response.StatusCode}. service answer: {errorContent}");
            }
        }
    }
    
}
