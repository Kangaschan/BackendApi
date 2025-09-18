using LRA.Subscriptions.Application.DTOs.Request;
using LRA.Subscriptions.Application.DTOs.Response;

namespace LRA.Subscriptions.Application.Interfaces;

public interface ISubscriptionsRepository
{
    public Task CreateAsync(SubscriptionCreateDto createDto, CancellationToken cancellationToken);
    public Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    public Task UpdateAsync(Guid? id, SubscriptionUpdateDto updateDto, CancellationToken cancellationToken);
    public Task<IEnumerable<SubscriptionResponseDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    public Task<SubscriptionResponseDto> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    public Task<SubscriptionResponseDto> GetByIdAsync(Guid? id, CancellationToken cancellationToken);
}
