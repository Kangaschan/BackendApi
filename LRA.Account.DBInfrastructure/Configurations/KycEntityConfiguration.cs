using LRA.Account.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LRA.Account.DBInfrastructure.Configurations;

public class KycEntityConfiguration : IEntityTypeConfiguration<KycEntity>
{
    public void Configure(EntityTypeBuilder<KycEntity> entity)
    {
        entity.HasKey(k => k.Id);

        entity.Property(k => k.Id).ValueGeneratedOnAdd();

        entity.Property(k => k.CreatedAtUtc).IsRequired().HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()");
        
        entity.Property(k => k.UpdatedAtUtc).IsRequired().HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()");

        entity.Property(k => k.IdentityDocumentPhoto).IsRequired().HasMaxLength(255);

        entity.Property(k => k.IdentityDocumentSelfie).IsRequired().HasMaxLength(255);

        entity.Property(k => k.MedicalCertificatePhoto).IsRequired().HasMaxLength(255);

        entity.Property(k => k.AccountId).IsRequired();

        entity.Property(k => k.AdminReviewId).IsRequired(false);

        entity.Property(k => k.RejectReason).IsRequired(false).HasMaxLength(255);

        entity.Property(k => k.Status).HasConversion<string>().IsRequired();
        
        entity.HasOne<AccountEntity>()
            .WithMany()
            .HasForeignKey(k => k.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
