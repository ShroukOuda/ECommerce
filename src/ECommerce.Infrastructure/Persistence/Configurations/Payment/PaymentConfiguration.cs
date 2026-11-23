using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Payment;

public class PaymentConfiguration : IEntityTypeConfiguration<Domain.Entities.Payments.Payment>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Payments.Payment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.TransactionId).IsUnique();
        builder.HasIndex(p => p.StripeCheckoutSessionId)
            .IsUnique()
            .HasFilter("[StripeCheckoutSessionId] IS NOT NULL");
        builder.HasIndex(p => p.StripePaymentIntentId)
            .IsUnique()
            .HasFilter("[StripePaymentIntentId] IS NOT NULL");

        builder.Property(p => p.TransactionId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Currency)
            .HasMaxLength(10);

        builder.Property(p => p.PaymentGateway)
            .HasMaxLength(100);

        builder.Property(p => p.GatewayTransactionId)
            .HasMaxLength(200);

        builder.Property(p => p.GatewayResponse)
            .HasMaxLength(2000);

        builder.Property(p => p.StripeCheckoutSessionId)
            .HasMaxLength(200);

        builder.Property(p => p.StripeCheckoutSessionStatus)
            .HasMaxLength(50);

        builder.Property(p => p.StripeCheckoutSessionUrl)
            .HasMaxLength(2048);

        builder.Property(p => p.StripePaymentIntentId)
            .HasMaxLength(200);

        builder.Property(p => p.Status)
            .HasConversion<string>();

        builder.Property(p => p.Method)
            .HasConversion<string>();

        builder.Property(p => p.Amount)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.AmountInMinorUnits)
            .IsRequired();

        builder.Property(p => p.PaidAt)
            .IsRequired(false);

        builder.HasOne(p => p.Order)
            .WithMany(o => o.Payments)
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(p => p.User)
            .WithMany(u => u.Payments)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}