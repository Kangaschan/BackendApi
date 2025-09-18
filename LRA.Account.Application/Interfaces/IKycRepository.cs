using LRA.Account.Application.DTOs;
using LRA.Common.DTOs.KYC;

namespace LRA.Account.Application.Interfaces;

public interface IKycRepository
{
    Task CreateAsync(KycCreateRequest request, CancellationToken cancellationToken);
    Task<IEnumerable<KycListItemDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<KycDetailsDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<KycListItemDto>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken);
    Task UpdateAsync(Guid id, KycUpdateRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
