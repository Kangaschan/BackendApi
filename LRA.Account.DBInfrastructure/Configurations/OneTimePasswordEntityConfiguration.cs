using LRA.Account.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LRA.Account.DBInfrastructure.Configurations;

public class OneTimePasswordEntityConfiguration : IEntityTypeConfiguration<OneTimePasswordEntity>
{
    public void Configure(EntityTypeBuilder<OneTimePasswordEntity> entity)
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
        entity.Property(e => e.UserEmail).IsRequired().HasMaxLength(255);
        entity.Property(e => e.ExpiresAt).IsRequired().HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()");
        entity.Property(e => e.CreatedAtUtc).IsRequired().HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()");
        entity.Property(e => e.UpdatedAtUtc).IsRequired().HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()");
        entity.HasIndex(e => new { e.Password, e.UserEmail })
            .IsUnique();
    }
}
