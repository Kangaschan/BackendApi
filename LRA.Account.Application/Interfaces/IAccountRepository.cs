using LRA.Account.Application.DTOs;
using LRA.Account.Domain.Models;
using LRA.Common.DTOs;
using LRA.Common.DTOs.Auth;
using LRA.Common.Models;

namespace LRA.Account.Application.Interfaces;

public interface IAccountRepository
{
    Task<IEnumerable<AccountDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<AccountDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<AccountDto?> GetByKeycloakIdAsync(KeycloakId id, CancellationToken cancellationToken);
    Task<PagedResult<AccountSearchResult>> GetFilteredAsync(ViewFilterParams filterParams, CancellationToken cancellationToken);
    Task<PagedResult<AccountSearchResult>> SearchAsync(AccountSearchParams searchParams, CancellationToken cancellationToken);
    Task CreateAsync(AccountDto account, CancellationToken cancellationToken);
    Task UpdateAsync(Guid id,AccountDto account, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<AccountDto?> GetbyEmail(string email, CancellationToken cancellationToken);
    Task Update2FaAsync(KeycloakId keycloakId, Change2FaRequest сhange2FaRequest, CancellationToken cancellationToken);
    Task ChangeFullNameAsync(KeycloakId keycloakId, ChangeFullNameRequest changeFullNameRequest, CancellationToken cancellationToken);
    Task UpdateCredentialsAsync(string email, UpdateAccountCredentialsRequest request, CancellationToken cancellationToken);
    Task BlockAsync(Guid id, BlockRequest request, CancellationToken cancellationToken);
}
