using LRA.Account.Application.DTOs;
using LRA.Common.DTOs.Auth;
using LRA.Common.Models;

namespace LRA.Account.Application.Interfaces;

public interface IKeycloakServiceClient
{
    Task<KeycloakId> CreateAccountAsync(KeycloakAccountCreateRequest accountCreateRequest, CancellationToken cancellationToken);
    Task UpdateAccountAsync(KeycloakId id, KeycloakAccountUpdateRequest account, CancellationToken cancellationToken);
    Task DeleteAccountAsync(KeycloakId keycloakId, CancellationToken cancellationToken);
    Task ActivateAccountAsync(KeycloakId id, CancellationToken cancellationToken);
    Task<KeycloakTokenResponse> LoginAccountAsync(LoginRequest loginRequest, CancellationToken cancellationToken);
    Task<KeycloakTokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    Task ChangePasswordAsync(ChangePasswordRequest changePasswordRequest, KeycloakId keycloakId, CancellationToken cancellationToken);
    Task ResetPasswordAsync(string newPassword, KeycloakId keycloakId, CancellationToken cancellationToken);
    Task<JsonWebKeysList> GetJwksAsync(CancellationToken cancellationToken);
    Task CompleteEmailChangeAsync(KeycloakId keycloakId, string newEmail, CancellationToken cancellationToken);
    string GetGoogleAuthUrl();
    Task<KeycloakTokenResponse> ExchangeCodeForTokenAsync(string code, CancellationToken cancellationToken);
    Task<string> GetUserEmailByIdAsync(KeycloakId keycloakId, CancellationToken cancellationToken);
    Task<(KeycloakId, string)> GetUserInfoFromAccessTokenAsync(string accessToken, CancellationToken cancellationToken);
}
