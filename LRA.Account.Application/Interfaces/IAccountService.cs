using LRA.Account.Application.DTOs;
using LRA.Common.DTOs;
using LRA.Common.DTOs.Auth;
using LRA.Common.DTOs.KYC;
using LRA.Common.Models;
using ChangeEmailRequest = LRA.Account.Application.DTOs.ChangeEmailRequest;
using ChangePasswordRequest = LRA.Account.Application.DTOs.ChangePasswordRequest;
using CompleteResetPasswordRequest = LRA.Account.Application.DTOs.CompleteResetPasswordRequest;

namespace LRA.Account.Application.Interfaces;

public interface IAccountService
{
    Task<IEnumerable<AccountDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<AccountDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<AccountSearchResult>> SearchAsync(AccountSearchParams searchParams, CancellationToken cancellationToken);
    Task<PagedResult<AccountSearchResult>> GetFilteredAsync(ViewFilterParams filterParams, CancellationToken cancellationToken);
    Task CreateAsync(CreateAccountRequestDto account, CancellationToken cancellationToken);
    Task RegisterAsync(RegisterAccountDto account, CancellationToken cancellationToken);
    Task UpdateAsync(Guid id, AccountDto account, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ConfirmEmailAsync(TokenDto dto, CancellationToken cancellationToken);
    Task<(JwtTokenDto?, bool)> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken);
    Task<JwtTokenDto> RefreshToken(RefreshTokenDto refreshTokenDto, CancellationToken cancellationToken);
    Task<(JwtTokenDto?, bool)> Complete2FaAsync(Complete2FaRequest dto, CancellationToken cancellationToken);
    Task<JsonWebKeysList> GetJwksAsync(CancellationToken cancellationToken);
    Task ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest, CancellationToken cancellationToken);
    Task<bool>  CompletePasswordResetAsync(CompleteResetPasswordRequest completeResetPasswordRequest, CancellationToken cancellationToken);
    Task ChangePasswordAsync(string userMail, ChangePasswordRequest changePasswordRequest, CancellationToken cancellationToken);
    Task Update2FaAsync(string keycloakId, Change2FaRequest сhange2FaRequest, CancellationToken cancellationToken);
    Task ChangeFullNameAsync(string keycloakId, ChangeFullNameRequest changeFullNameRequest, CancellationToken cancellationToken);
    Task ChangeMailAsync(string keycloakId, ChangeEmailRequest changeEmailRequest, CancellationToken cancellationToken);
    Task<bool> CompleteChangeMailAsync(TokenDto dto, CancellationToken cancellationToken);
    Task<RedirectUrl> GetGoogleAuthUrl(CancellationToken cancellationToken);
    Task<JwtTokenDto> CompleteGoogleAuthAsync(string code, CancellationToken cancellationToken);
    Task<AccountRoles> GetAccountRolesAsync(string email, CancellationToken cancellationToken);
    Task<AccountRoles> GetAccountRolesByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AdminChangePasswordAsync(string email, AdminChangePasswordRequest request, CancellationToken cancellationToken);
    Task BlockAsync(Guid id, BlockRequest request, CancellationToken cancellationToken);
    Task ApplyForAJobAsync(string email, JobApplicationDto jobApplication, CancellationToken cancellationToken);
    Task<IEnumerable<KycListItemDto>> CheckKycStatusAsync(string email, CancellationToken cancellationToken);
    Task<IEnumerable<KycListItemDto>> GetAllJobApplicationsAsync(CancellationToken cancellationToken);
    Task<JobApplication> GetDetailedJobApplicationAsync(Guid id, CancellationToken cancellationToken);
    Task ProcessJobApplicationAsync(KycProcessDto kycProcessDto, CancellationToken cancellationToken);
}
