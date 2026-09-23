using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.Entities;

namespace POS.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.Id);

        // TotalPrice is a computed property (Quantity * UnitPrice), not a stored column.
        builder.Ignore(oi => oi.TotalPrice);

        // Many OrderItems reference one Product. Don't allow deleting a Product that
        // is still referenced by historical order items — that would corrupt past orders.
        builder.HasOne(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // The Order side of this relationship (HasMany/WithOne/HasForeignKey) is already
        // configured in OrderConfiguration — EF Core only needs it declared once, from either side.
    }
}
