using LRA.Account.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LRA.Account.DBInfrastructure.Data;

public interface IAppDbContext
{
    public DbSet<AccountEntity> Accounts { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<EmailConfirmationTokenEntity> ConfirmationTokens { get; set; }
    public DbSet<OneTimePasswordEntity> OneTinePasswords { get; set; }
    public DbSet<CredentialsDetailsEntity> CredentialsDetails { get; set; }
    public DbSet<KycEntity> Kycs { get; set; }

    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
