using ECommerce.Domain.Entities.Payments;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Payment;

public class PaymentStatusHistoryConfiguration : IEntityTypeConfiguration<PaymentStatusHistory>
{
    public void Configure(EntityTypeBuilder<PaymentStatusHistory> builder)
    {
        builder.HasKey(h => h.Id);

        builder.Property(h => h.PaymentStatus)
            .HasConversion<string>();

        builder.Property(h => h.Source)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(h => h.Reason)
            .HasMaxLength(500);

        builder.HasOne(h => h.Payment)
            .WithMany(p => p.StatusHistories)
            .HasForeignKey(h => h.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
