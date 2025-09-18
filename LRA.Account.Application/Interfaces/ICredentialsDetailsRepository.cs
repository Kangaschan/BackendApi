namespace LRA.Account.Application.Interfaces;

public interface ICredentialsDetailsRepository
{
    Task CreateCredentialAsync( CancellationToken cancellationToken);
    Task GetCredentialByAccountIdAsync(Guid accountId, CancellationToken cancellationToken);
    Task UpdateCredentialAsync( Guid credentialId, CancellationToken cancellationToken);
}
