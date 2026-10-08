using Inventory.Domain.Common;


namespace Inventory.Domain.Entities;

public class PurchaseOrderItem : EntityBase
{
    public Guid PurchaseOrderId { get; private set; }
    public PurchaseOrder PurchaseOrder { get; private set; }

    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    private PurchaseOrderItem() { PurchaseOrder = null!; }

    public PurchaseOrderItem(Guid purchaseOrderId, Guid productId, int quantity, decimal unitPrice)
    {
        PurchaseOrderId = purchaseOrderId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public void UpdateDetails(int newQuantity, decimal newUnitPrice)
    {
        Quantity = newQuantity;
        UnitPrice = newUnitPrice;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = "System";
    }
}