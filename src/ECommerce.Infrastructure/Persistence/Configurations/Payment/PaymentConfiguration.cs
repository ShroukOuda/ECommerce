using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Payment;

public class PaymentConfiguration : IEntityTypeConfiguration<Core.Entities.Payment.Payment>
{
    public void Configure(EntityTypeBuilder<Core.Entities.Payment.Payment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.TransactionId).IsUnique();

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

        builder.Property(p => p.Status)
            .HasConversion<string>();

        builder.Property(p => p.Method)
            .HasConversion<string>();

        builder.Property(p => p.Amount)
            .HasColumnType("decimal(18,2)");

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