using ECommerce.Domain.Entities.Returns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Return;

public class ReturnHistoryConfiguration : IEntityTypeConfiguration<ReturnHistory>
{
    public void Configure(EntityTypeBuilder<ReturnHistory> builder)
    {
        builder.HasKey(rh => rh.Id);

        builder.Property(rh => rh.Action)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(rh => rh.PerformedById)
            .IsRequired();

        builder.Property(rh => rh.CreatedAt)
            .IsRequired();

        // ReturnRequest → ReturnHistory
        builder.HasOne(rh => rh.ReturnRequest)
            .WithMany(rr => rr.ReturnHistories)
            .HasForeignKey(rh => rh.ReturnRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        // User → ReturnHistory
        builder.HasOne(rh => rh.PerformedBy)
            .WithMany()
            .HasForeignKey(rh => rh.PerformedById)
            .OnDelete(DeleteBehavior.NoAction);


        builder.HasIndex(rh => rh.ReturnRequestId);
    }
}