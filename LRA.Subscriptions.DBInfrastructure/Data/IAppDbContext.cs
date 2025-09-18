using LRA.Subscriptions.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LRA.Subscriptions.DBInfrastructure.Data;

public interface IAppDbContext
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<SubscriptionEntity> Subscriptions { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
