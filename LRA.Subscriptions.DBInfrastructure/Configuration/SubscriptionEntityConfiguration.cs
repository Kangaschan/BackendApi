using LRA.Subscriptions.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LRA.Subscriptions.DBInfrastructure.Configuration;

public class SubscriptionEntityConfiguration: IEntityTypeConfiguration<SubscriptionEntity>
{
    public void Configure(EntityTypeBuilder<SubscriptionEntity> entity)
    {
        entity.Property(e => e.Id)
            .HasColumnType("uuid")
            .ValueGeneratedOnAdd();

        entity.Property(e => e.StripeSubscriptionId)
            .IsRequired()
            .HasMaxLength(255);

        entity.Property(e => e.Status)
            .HasMaxLength(50);

        entity.Property(e => e.PriceId)
            .IsRequired()
            .HasMaxLength(255);

        entity.Property(e => e.CurrentPeriodStart)
            .HasColumnType("timestamp with time zone");

        entity.Property(e => e.CurrentPeriodEnd)
            .HasColumnType("timestamp with time zone");

        entity.Property(e => e.CancelAtPeriodEnd)
            .HasColumnType("boolean");

        entity.Property(e => e.CreatedAtUtc)
            .HasColumnType("timestamp with time zone");
        
        entity.Property(e => e.UpdatedAtUtc)
            .HasColumnType("timestamp with time zone");

        entity.HasIndex(e => e.StripeSubscriptionId)
            .IsUnique();
    }
}
