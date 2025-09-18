using LRA.Account.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LRA.Account.DBInfrastructure.Seeds.Configurations;

public class RoleSeedConfiguration : IEntityTypeConfiguration<RoleEntity>
{
    private static readonly DateTime InitTime = new DateTime(2023, 12, 25, 15, 30, 0, DateTimeKind.Utc)
        ;

    public void Configure(EntityTypeBuilder<RoleEntity> builder)
    {
        builder.HasData(
            new RoleEntity { Id = Guid.Parse("766745d4-137e-42d5-b8b5-73d039a91720"), Name = "admin", CreatedAtUtc = InitTime, UpdatedAtUtc = InitTime },
            new RoleEntity { Id = Guid.Parse("410434b8-9c7d-47be-a591-2bc1e6005d24"), Name = "superAdmin", CreatedAtUtc = InitTime, UpdatedAtUtc = InitTime },
            new RoleEntity { Id = Guid.Parse("85b823f0-4b6a-45fe-a44b-efd0df385ace"), Name = "paramedic", CreatedAtUtc = InitTime, UpdatedAtUtc = InitTime },
            new RoleEntity { Id = Guid.Parse("d6a978a8-aa11-471c-be82-71ef221afdf0"), Name = "client", CreatedAtUtc = InitTime, UpdatedAtUtc = InitTime }
        );
    }
}
