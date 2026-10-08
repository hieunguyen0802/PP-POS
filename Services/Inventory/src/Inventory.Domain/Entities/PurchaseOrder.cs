using Inventory.Domain.Common;

namespace Inventory.Domain.Entities;


public class PurchaseOrder : AggregateRoot
{
    public string OrderNumber { get; private set; }
    public DateTime OrderDate { get; private set; }
    public Guid SupplierId { get; private set; }
    public Supplier Supplier { get; private set; }
    public PurchaseOrderStatus Status { get; private set; }

    private readonly List<PurchaseOrderItem> _items = new();
    public IReadOnlyCollection<PurchaseOrderItem> Items => _items.AsReadOnly();

    private PurchaseOrder() { OrderNumber = null!; Supplier = null!; }

    public PurchaseOrder(string orderNumber, DateTime orderDate, Guid supplierId)
    {
        OrderNumber = orderNumber;
        OrderDate = orderDate;
        SupplierId = supplierId;
        Status = PurchaseOrderStatus.Draft;
    }

    public void UpdateOrderDetails(string newOrderNumber, DateTime newOrderDate)
    {
        if (Status != PurchaseOrderStatus.Draft)
            throw new InvalidOperationException("Cannot edit a purchase order that is no longer in Draft status.");

        OrderNumber = newOrderNumber;
        OrderDate = newOrderDate;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = "System";
    }

    public void AddItem(Guid productId, int quantity, decimal unitPrice)
    {
        if (Status != PurchaseOrderStatus.Draft)
            throw new InvalidOperationException("Cannot add items to a purchase order that is no longer in Draft status.");

        _items.Add(new PurchaseOrderItem(Id, productId, quantity, unitPrice));
    }

    public void MarkAsOrdered()
    {
        if (Status != PurchaseOrderStatus.Draft)
            throw new InvalidOperationException("Only a Draft purchase order can be marked as Ordered.");
        if (_items.Count == 0)
            throw new InvalidOperationException("Cannot order a purchase order with no items.");

        Status = PurchaseOrderStatus.Ordered;
    }

    public void MarkAsReceived()
    {
        if (Status != PurchaseOrderStatus.Ordered)
            throw new InvalidOperationException("Only an Ordered purchase order can be marked as Received.");

        Status = PurchaseOrderStatus.Received;
    }

    public void Cancel()
    {
        if (Status == PurchaseOrderStatus.Received)
            throw new InvalidOperationException("Cannot cancel a purchase order that has already been Received.");

        Status = PurchaseOrderStatus.Cancelled;
    }
}