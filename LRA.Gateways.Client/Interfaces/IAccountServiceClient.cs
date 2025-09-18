using LRA.Common.DTOs;
using LRA.Common.DTOs.Auth;
using LRA.Common.DTOs.KYC;
using LRA.Common.Models;
using LRA.Gateways.Client.DTOs;

namespace LRA.Gateways.Client.Interfaces;

public interface IAccountServiceClient
{
    Task RegisterAsync(RegisterAccountRequestDto accountRequest, CancellationToken cancellationToken);
    Task VerifyAsync(TokenDto token, CancellationToken cancellationToken);
    Task ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest, CancellationToken cancellationToken);
    Task CompletePasswordResetAsync(CompleteResetPasswordRequest completeResetPasswordRequest, CancellationToken cancellationToken);
    Task ChangePasswordAsync(string mail, ChangePasswordRequest changePasswordRequest, CancellationToken cancellationToken);
    Task<JwtTokenDto> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken);
    Task<JwtTokenDto> RefreshAsync(RefreshTokenDto refreshToken, CancellationToken cancellationToken);
    Task ChangeMailAsync(string keycloakId, ChangeEmailRequest changeEmailRequest, CancellationToken cancellationToken);
    Task<bool> CompleteChangeMailAsync(TokenDto token, CancellationToken cancellationToken);
    Task ChangeFullNameAsync(string keycloakId, ChangeFullNameRequest changeFullNameRequest, CancellationToken cancellationToken);
    Task Change2FaAsync(string keycloakId, Change2FaRequest change2FaRequest, CancellationToken cancellationToken);
    Task<(JwtTokenDto, bool)> Complete2FaAsync(Complete2FaRequest сomplete2FaRequest, CancellationToken cancellationToken);
    Task <RedirectUrl> GoogleLoginAsync(CancellationToken cancellationToken);
    Task<JwtTokenDto> CompleteGoogleLoginAsync(string code, CancellationToken cancellationToken);
    Task<JsonWebKeysList> GetJsonWebKeyAsync(CancellationToken cancellationToken);
    Task ApplyForAJobAsync(string email, JobApplicationDto jobApplication, CancellationToken cancellationToken);
    Task<IEnumerable<KycListItemDto>?> CheckKycStatusAsync(string email, CancellationToken cancellationToken);
}
