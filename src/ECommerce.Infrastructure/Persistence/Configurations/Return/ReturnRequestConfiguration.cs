using ECommerce.Domain.Entities.Returns;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Return;

public class ReturnRequestConfiguration : IEntityTypeConfiguration<ReturnRequest>
{
    public void Configure(EntityTypeBuilder<ReturnRequest> builder)
    {
        builder.HasKey(rr => rr.Id);

        builder.HasIndex(rr => rr.ReturnNumber).IsUnique();

        builder.Property(rr => rr.ReturnNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(rr => rr.RejectionReason)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(rr => rr.Description)
            .HasMaxLength(2000);

        builder.Property(rr => rr.Status)
            .IsRequired();

        builder.Property(rr => rr.RefundAmount)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(rr => rr.Order)
            .WithMany(o => o.ReturnRequests)
            .HasForeignKey(rr => rr.OrderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(rr => rr.User)
            .WithMany(u => u.ReturnRequests)
            .HasForeignKey(rr => rr.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(rr => rr.ReturnItems)
            .WithOne(ri => ri.ReturnRequest)
            .HasForeignKey(ri => ri.ReturnRequestId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(rr => rr.ReturnHistories)
            .WithOne(rh => rh.ReturnRequest)
            .HasForeignKey(rh => rh.ReturnRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}