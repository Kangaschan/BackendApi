using LRA.Account.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LRA.Account.DBInfrastructure.Configurations;

public class EmailConfirmationTokenEntityConfiguration : IEntityTypeConfiguration<EmailConfirmationTokenEntity>
{
    public void Configure(EntityTypeBuilder<EmailConfirmationTokenEntity> entity)
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Token).IsRequired().HasMaxLength(255);
        entity.Property(e => e.UserEmail).IsRequired().HasMaxLength(255);
        entity.Property(e => e.ExpiresAt).IsRequired().HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()");
        entity.Property(e => e.CreatedAtUtc).IsRequired().HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()");
        entity.Property(e => e.UpdatedAtUtc).IsRequired().HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()");
        entity.Property(e => e.IsUsed).IsRequired().HasDefaultValue(false);
        entity.HasIndex(e => new { e.Token, e.UserEmail }).IsUnique();
    }
}
