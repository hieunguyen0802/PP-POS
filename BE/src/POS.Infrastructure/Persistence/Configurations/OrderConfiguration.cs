using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.Entities;

namespace POS.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.HasIndex(o => o.OrderNumber).IsUnique();

        // One Order has many OrderItems, each OrderItem belongs to exactly one Order.
        // Deleting an Order deletes its OrderItems too (they have no independent lifecycle).
        builder.HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Many Orders belong to one Customer. An order always has a customer (CustomerId is a Guid, not Guid?),
        // so this is a required relationship; don't allow deleting a Customer that still has Orders.
        builder.HasOne(o => o.Customer)
            .WithMany()
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
