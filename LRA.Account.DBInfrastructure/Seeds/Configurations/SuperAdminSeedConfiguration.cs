using LRA.Account.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LRA.Account.DBInfrastructure.Seeds.Configurations;

public class SuperAdminSeedConfiguration : IEntityTypeConfiguration<AccountEntity>
{
    public void Configure(EntityTypeBuilder<AccountEntity> builder)
    {
        builder.HasData(
            new AccountEntity
            {
                Id = Guid.Parse("cb67ce07-ad2c-4d16-9238-31c10b60306a"),
                KeycloakId = "f0b5af18-5352-4716-9ef4-c6654e5e7aeb",
                Email = "superadmin@mail.com",
                IsBlocked = false,
                IsTwoFactorEnabled = false,
                CreatedAtUtc = new DateTime(2023, 12, 25, 15, 30, 0, DateTimeKind.Utc),
                UpdatedAtUtc = new DateTime(2023, 12, 25, 15, 30, 0, DateTimeKind.Utc),
                IsDeleted = false,
            }
        );
    }
}
