using LRA.Account.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LRA.Account.DBInfrastructure.Configurations;

public class AccountEntityConfiguration : IEntityTypeConfiguration<AccountEntity>
{
    public void Configure(EntityTypeBuilder<AccountEntity> entity)
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();
        entity.Property(e => e.KeycloakId).IsRequired().HasMaxLength(255);
        entity.Property(e => e.FirstName).HasMaxLength(100);
        entity.Property(e => e.LastName).HasMaxLength(100);
        entity.Property(e => e.Phone).HasMaxLength(20);
        entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
        entity.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);
        entity.Property(e => e.UpdatedAtUtc).IsRequired().HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()");
        entity.Property(e => e.CreatedAtUtc).IsRequired().HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()");
        entity.Property(e => e.BlockedUntil).HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()");
        entity.Property(e => e.IsBlocked).IsRequired().HasDefaultValue(false);
        entity.Property(e => e.IsTwoFactorEnabled).IsRequired().HasDefaultValue(true);

        entity.HasMany(a => a.Roles)
            .WithMany(r => r.Accounts)
            .UsingEntity<Dictionary<string, object>>(
                "AccountRoles",
                j => j.HasOne<RoleEntity>().WithMany().HasForeignKey("RolesId"),
                j => j.HasOne<AccountEntity>().WithMany().HasForeignKey("AccountsId"),
                j =>
                {
                    j.HasKey("AccountsId", "RolesId");
                    j.HasData(
                        new
                        {
                            AccountsId = Guid.Parse("cb67ce07-ad2c-4d16-9238-31c10b60306a"),
                            RolesId = Guid.Parse("410434b8-9c7d-47be-a591-2bc1e6005d24")
                        }
                    );
                }
            );
    }
}
