using LRA.Account.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LRA.Account.DBInfrastructure.Configurations;

public class RoleEntityConfiguration : IEntityTypeConfiguration<RoleEntity>
{
    public void Configure(EntityTypeBuilder<RoleEntity> entity)
    {
        entity.HasKey(r => r.Id);
        entity.Property(r => r.Name).IsRequired().HasMaxLength(20);
        entity.Property(r => r.CreatedAtUtc).IsRequired().HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()");
        entity.Property(r => r.UpdatedAtUtc).IsRequired().HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()");
    }
}
