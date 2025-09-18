using LRA.Account.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LRA.Account.DBInfrastructure.Configurations;

public class CredentialsDetailsEntityConfiguration : IEntityTypeConfiguration<CredentialsDetailsEntity>
{
    public void Configure(EntityTypeBuilder<CredentialsDetailsEntity> entity)
    {
        entity.HasKey(e => e.Id);
        
        entity.Property(e => e.IsUsed).IsRequired().HasDefaultValue(false);
            
        entity.Property(e => e.IsTemporary).IsRequired().HasDefaultValue(false);
            
        entity.Property(e => e.ExpiresAt).HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()");
        entity.Property(e => e.UpdatedAtUtc).HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()");
        entity.Property(e => e.CreatedAtUtc).HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()");
            
        entity.HasOne(e => e.Account)
            .WithOne(a => a.Credentials)
            .HasForeignKey<CredentialsDetailsEntity>(e => e.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
        
        entity.HasData(
            new CredentialsDetailsEntity
            {
                Id = Guid.Parse("d4f7b2e2-7b8e-4b7b-8f1a-6e7c9d8e7f9a"),
                AccountId = Guid.Parse("cb67ce07-ad2c-4d16-9238-31c10b60306a"),
                IsUsed = false,
                IsTemporary = false,
                ExpiresAt = null,
                CreatedAtUtc = new DateTime(2023, 12, 25, 15, 30, 0, DateTimeKind.Utc),
                UpdatedAtUtc = new DateTime(2023, 12, 25, 15, 30, 0, DateTimeKind.Utc),
            }
        );
    }
}
