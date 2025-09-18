using LRA.Subscriptions.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LRA.Subscriptions.DBInfrastructure.Configuration;

public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> entity)
    {
        entity.HasKey(e => e.Id);
        
        entity.Property(e => e.Id)
            .HasColumnType("uuid")
            .ValueGeneratedOnAdd();
            
        entity.Property(e => e.StripeCustomerId)
            .IsRequired()
            .HasMaxLength(255);

        entity.Property(e => e.UserEmail)
            .IsRequired()
            .HasMaxLength(255);

        entity.Property(e => e.CreatedAtUtc)
            .HasColumnType("timestamp with time zone");
        
        entity.Property(e => e.UpdatedAtUtc)
            .HasColumnType("timestamp with time zone");

        entity.HasOne(u => u.CurrentSubscription)
            .WithOne(s => s.User)
            .HasForeignKey<SubscriptionEntity>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
