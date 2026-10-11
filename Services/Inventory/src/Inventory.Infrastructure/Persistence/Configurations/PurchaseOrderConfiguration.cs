
using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.Configurations;

public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.OrderNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(p => p.OrderNumber).IsUnique();

        // Store the status as readable text ("Draft") instead of a number (0).
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);

        // Same database, so a real FK to Supplier is fine. Don't allow deleting a supplier that has orders.
        builder.HasOne(p => p.Supplier)
            .WithMany()
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        // Items belong to the order: deleting the order deletes its items.
        builder.HasMany(p => p.Items)
            .WithOne(i => i.PurchaseOrder)
            .HasForeignKey(i => i.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Items is a read-only view over the private _items list.
        // Tell EF to fill the field directly, so callers can never bypass AddItem().
        builder.Navigation(p => p.Items).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(p => p.RowVersion).IsRowVersion();
    }
}