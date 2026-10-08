using Inventory.Domain.Common;
using Inventory.Domain.Exceptions;

namespace Inventory.Domain.Entities;

public class InventoryItem : EntityBase
{
    public Guid ProductId { get; private set; }
    public int QuantityOnHand { get; private set; }
    public int ReorderThreshold { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();
    private InventoryItem() { } // For EF Core

    public InventoryItem(Guid productId, int initialQuantity, int reorderThreshold)
    {
        if (initialQuantity < 0)
            throw new ArgumentException("Initial quantity cannot be negative.");

        ProductId = productId;
        QuantityOnHand = initialQuantity;
        ReorderThreshold = reorderThreshold;
    }

    public void Receive(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity to receive must be positive.");

        QuantityOnHand += quantity;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = "System";
    }

    public void Reserve(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity to reserve must be positive.");
        if (quantity > QuantityOnHand)
            throw new DomainExceptions.InsufficientStock(ProductId, quantity, QuantityOnHand);

        QuantityOnHand -= quantity;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = "System";
    }

    public void Release(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity to release must be positive.");

        QuantityOnHand += quantity;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = "System";
    }

    public void Adjust(int delta)
    {
        var newQuantity = QuantityOnHand + delta;
        if (newQuantity < 0)
            throw new InvalidOperationException("Adjustment would result in negative stock.");

        QuantityOnHand = newQuantity;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = "System";
    }

    public bool IsBelowReorderThreshold() => QuantityOnHand < ReorderThreshold;
}