using Inventory.Domain.Common;

namespace Inventory.Domain.Entities;

public class StockAdjustment : EntityBase
{
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public string Reason { get; private set; }
    private StockAdjustment() { Reason = null!; }

    public StockAdjustment(Guid productId, int quantity, string reason)
    {
        ProductId = productId;
        Quantity = quantity;
        Reason = reason;
    }

    public void UpdateDetails(int newQuantity, string newReason)
    {
        Quantity = newQuantity;
        Reason = newReason;

    }
}