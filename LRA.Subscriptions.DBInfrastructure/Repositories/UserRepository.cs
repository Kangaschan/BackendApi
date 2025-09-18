using LRA.Subscriptions.Application.DTOs;
using LRA.Subscriptions.Application.DTOs.Request;
using LRA.Subscriptions.Application.DTOs.Response;
using LRA.Subscriptions.Application.Interfaces;
using LRA.Subscriptions.DBInfrastructure.Data;
using LRA.Subscriptions.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LRA.Subscriptions.DBInfrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IAppDbContext _context;

    public UserRepository(IAppDbContext context)
    {
        _context = context;
    }

    public Task<UserResopnseDto?> GetByMailAsync(string mail, CancellationToken cancellationToken)
    {
        var user = _context.Users
            .AsNoTracking()
            .Where(user => user.UserEmail == mail)
            .Select(user => new UserResopnseDto
            {
                Id = user.Id,
                StripeCustomerId = user.StripeCustomerId,
                UserEmail = user.UserEmail,
                CreatedAt = user.CreatedAtUtc,
                CurrentSubscription = user.CurrentSubscription.Id
            })
            .FirstOrDefaultAsync(cancellationToken);
        
        return user;
    }

    public Task<UserResopnseDto?> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken)
    {
        var user = _context.Users
            .AsNoTracking()
            .Where(user => user.StripeCustomerId == customerId)
            .Select(user => new UserResopnseDto
            {
                Id = user.Id,
                StripeCustomerId = user.StripeCustomerId,
                UserEmail = user.UserEmail,
                CreatedAt = user.CreatedAtUtc,
                CurrentSubscription = user.CurrentSubscription.Id
            })
            .FirstOrDefaultAsync(cancellationToken);
        
        return user;
    }

    public async Task CreateUserAsync(UserCreateDto user, CancellationToken cancellationToken)
    {
        var userEntity = new UserEntity
        {
            UserEmail = user.UserEmail,
            StripeCustomerId = user.StripeCustomerId,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
        };
        _context.Users.Add(userEntity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
