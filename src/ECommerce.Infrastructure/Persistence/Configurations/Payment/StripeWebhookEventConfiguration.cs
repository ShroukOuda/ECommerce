using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Payment;

public class StripeWebhookEventConfiguration : IEntityTypeConfiguration<Domain.Entities.Payments.StripeWebhookEvent>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Payments.StripeWebhookEvent> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.EventId).IsUnique();

        builder.Property(e => e.EventId)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.EventType)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Provider)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.StripeCheckoutSessionId)
            .HasMaxLength(200);

        builder.Property(e => e.StripePaymentIntentId)
            .HasMaxLength(200);

        builder.Property(e => e.ProcessingResult)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.ErrorMessage)
            .HasMaxLength(2000);

        builder.Property(e => e.ProcessedAt)
            .IsRequired();

        builder.HasOne(e => e.Payment)
            .WithMany()
            .HasForeignKey(e => e.PaymentId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}