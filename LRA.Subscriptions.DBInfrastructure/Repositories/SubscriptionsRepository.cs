using LRA.Subscriptions.Application.DTOs.Request;
using LRA.Subscriptions.Application.DTOs.Response;
using LRA.Subscriptions.Application.Interfaces;
using LRA.Subscriptions.DBInfrastructure.Data;
using LRA.Subscriptions.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LRA.Subscriptions.DBInfrastructure.Repositories;

public class SubscriptionsRepository : ISubscriptionsRepository
{
    private readonly IAppDbContext _context;

    public SubscriptionsRepository(IAppDbContext context)
    {
        _context = context;
    }
    
    public async Task CreateAsync(SubscriptionCreateDto createDto, CancellationToken cancellationToken)
    {
        var subscriptionEntity = new SubscriptionEntity
        {
            PriceId = createDto.PriceId,
            StripeSubscriptionId = createDto.StripeSubscriptionId,
            Status = createDto.Status,
            CurrentPeriodStart = createDto.CurrentPeriodStart,
            CurrentPeriodEnd = createDto.CurrentPeriodEnd,
            CancelAtPeriodEnd = createDto.CancelAtPeriodEnd,
            UserId = createDto.UserId,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
        };
        
        _context.Subscriptions.Add(subscriptionEntity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deletedSubscription = await _context.Subscriptions
            .Where(s => s.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        if (deletedSubscription == 0)
        {
            throw new Exception($"Subscription with id {id} not found");
        }
        
    }

    public async Task UpdateAsync(Guid? id, SubscriptionUpdateDto updateDto, CancellationToken cancellationToken)
    {
        var subscriptionUpdated = await _context.Subscriptions
            .Where(s => s.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(s => s.Status, updateDto.Status)
                .SetProperty(s => s.CurrentPeriodEnd, updateDto.CurrentPeriodEnd)
                .SetProperty(s => s.CurrentPeriodStart, updateDto.CurrentPeriodStart)
                .SetProperty(s => s.CancelAtPeriodEnd, updateDto.CancelAtPeriodEnd)
                .SetProperty(s => s.UpdatedAtUtc, DateTime.UtcNow)
                .SetProperty(s => s.PriceId, updateDto.PriceId),
                cancellationToken);
      
        if (subscriptionUpdated == 0)
        {
            throw new KeyNotFoundException($"Subscription with ID {id} not found");
        }
    }

    public async Task<IEnumerable<SubscriptionResponseDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var subscriptions = _context.Subscriptions
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .Select(subscription => new SubscriptionResponseDto
            {
                Id = subscription.Id,
                PriceId = subscription.PriceId,
                StripeSubscriptionId = subscription.StripeSubscriptionId,
                Status = subscription.Status,
                CurrentPeriodStart = subscription.CurrentPeriodStart,
                CurrentPeriodEnd = subscription.CurrentPeriodEnd,
                CancelAtPeriodEnd = subscription.CancelAtPeriodEnd,
                CreatedAt = subscription.CreatedAtUtc,
                UserId = subscription.UserId
            });
        return subscriptions;
    }

    public async Task<SubscriptionResponseDto> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var subscription = await _context.Subscriptions
            .AsNoTracking()
            .Where(s => s.UserId == userId && s.Status == "Active")
            .Select(subscription => new SubscriptionResponseDto
            {
                Id = subscription.Id,
                PriceId = subscription.PriceId,
                StripeSubscriptionId = subscription.StripeSubscriptionId,
                Status = subscription.Status,
                CurrentPeriodStart = subscription.CurrentPeriodStart,
                CurrentPeriodEnd = subscription.CurrentPeriodEnd,
                CancelAtPeriodEnd = subscription.CancelAtPeriodEnd,
                CreatedAt = subscription.CreatedAtUtc,
                UserId = subscription.UserId
            }).FirstOrDefaultAsync(cancellationToken);
        
        if (subscription == null)
        {
            throw new KeyNotFoundException($"Active subscription with user ID {userId} not found");
        }
        return subscription;
    }

    public async Task<SubscriptionResponseDto> GetByIdAsync(Guid? id, CancellationToken cancellationToken)
    {
        var subscription = await _context.Subscriptions
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(subscription => new SubscriptionResponseDto
            {
                Id = subscription.Id,
                PriceId = subscription.PriceId,
                StripeSubscriptionId = subscription.StripeSubscriptionId,
                Status = subscription.Status,
                CurrentPeriodStart = subscription.CurrentPeriodStart,
                CurrentPeriodEnd = subscription.CurrentPeriodEnd,
                CancelAtPeriodEnd = subscription.CancelAtPeriodEnd,
                CreatedAt = subscription.CreatedAtUtc,
                UserId = subscription.UserId
            }).FirstOrDefaultAsync(cancellationToken);
        
        if (subscription == null)
        {
            throw new KeyNotFoundException($"Subscription with ID {id} not found");
        }
        return subscription;
    }
}
