using LRA.Common.DTOs;
using LRA.Common.DTOs.Auth;
using LRA.Common.DTOs.KYC;
using LRA.Common.Models;
using LRA.Gateways.Admin.DTOs;
using Microsoft.AspNetCore.Identity.Data;
using LoginRequest = LRA.Common.DTOs.Auth.LoginRequest;
using ResetPasswordRequest = LRA.Gateways.Admin.DTOs.ResetPasswordRequest;

namespace LRA.Gateways.Admin.Interfaces;

public interface IAccountServiceClient
{
    Task<JwtTokenDto> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken);
    Task<JwtTokenDto> RefreshAsync(RefreshTokenDto refreshToken, CancellationToken cancellationToken);
    Task<JsonWebKeysList> GetJsonWebKeyAsync(CancellationToken cancellationToken);
    Task ChangeFullNameAsync(string keycloakId, ChangeFullNameRequest changeFullNameRequest, CancellationToken cancellationToken);
    Task Change2FaAsync(string keycloakId, Change2FaRequest change2FaRequest, CancellationToken cancellationToken);
    Task<(JwtTokenDto, bool)> Complete2FaAsync(Complete2FaRequest сomplete2FaRequest, CancellationToken cancellationToken);
    Task<AccountRolesResponse> CheckAdminRolesByEmail(string email, CancellationToken cancellationToken);
    Task<AccountRolesResponse> CheckAdminRolesById(Guid id, CancellationToken cancellationToken);
    Task ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest, CancellationToken cancellationToken);
    Task CompletePasswordResetAsync(CompleteResetPasswordRequest completeResetPasswordRequest, CancellationToken cancellationToken);
    Task ChangePasswordAsync(ChangePasswordRequest changePasswordRequest, CancellationToken cancellationToken);
    Task ChangeMailAsync(string keycloakId, ChangeEmailRequest changeEmailRequest, CancellationToken cancellationToken);
    Task<bool> CompleteChangeMailAsync(TokenDto token, CancellationToken cancellationToken);
    Task<CreateAccountRequestDto> CreateAccountAsync(AdminCreateAccountRequest request, CancellationToken cancellationToken);
    Task<PagedResult<AccountSearchResult>?> SearchAsync(AccountSearchParams searchParams, CancellationToken cancellationToken);
    Task<PagedResult<AccountSearchResult>?> GetFilteredAsync(ViewFilterParams filterParams, CancellationToken cancellationToken);
    Task<AccountDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateAccountAsync(Guid id, AccountDto updatedAccount, CancellationToken cancellationToken);
    Task<string> ResetAccountPasswordAsync(string email, ResetOptions resetOptions, CancellationToken cancellationToken);
    Task ChangeBlockStatus(Guid id, BlockRequest blockRequest, CancellationToken cancellationToken);
    Task DeleteAccountAsync(Guid id, CancellationToken cancellationToken);
    Task SoftDeleteAccountAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<KycListItemDto>?> GetAllJobApplicationsAsync(CancellationToken cancellationToken);
    Task<JobApplication?> GetDetailedJobApplicationAsync(Guid id, CancellationToken cancellationToken);
    Task ProcessJobApplicationAsync(JobApplicationProcessDto jobApplication, CancellationToken cancellationToken);
}
