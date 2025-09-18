using System.Reflection;
using LRA.Account.DBInfrastructure.Seeds;
using LRA.Account.Domain.Models;
using LRA.Common.DBFilters;
using Microsoft.EntityFrameworkCore;

namespace LRA.Account.DBInfrastructure.Data;

public class AppDbContext : DbContext, IAppDbContext
{
    public DbSet<AccountEntity> Accounts { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<EmailConfirmationTokenEntity> ConfirmationTokens { get; set; }
    public DbSet<OneTimePasswordEntity> OneTinePasswords { get; set; }
    public DbSet<CredentialsDetailsEntity> CredentialsDetails { get; set; }
    public DbSet<KycEntity> Kycs { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyGlobalFilters(new SoftDeleteFilter());
        
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    
    public override int SaveChanges()
    {
        this.HandleSoftDelete();
        return base.SaveChanges();
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        this.HandleSoftDelete();
        return base.SaveChangesAsync(cancellationToken);
    }
}
