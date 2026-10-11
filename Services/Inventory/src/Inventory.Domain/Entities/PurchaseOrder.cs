using Inventory.Domain.Common;
using Inventory.Domain.Exceptions;

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

    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

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
            throw new DomainExceptions.InvalidPurchaseOrderStatus(Status.ToString(), nameof(PurchaseOrderStatus.Ordered));
        if (_items.Count == 0)
            throw new InvalidOperationException("Cannot order a purchase order with no items.");

        Status = PurchaseOrderStatus.Ordered;
    }

    public void MarkAsReceived()
    {
        if (Status != PurchaseOrderStatus.Ordered)
            throw new DomainExceptions.InvalidPurchaseOrderStatus(Status.ToString(), nameof(PurchaseOrderStatus.Received));

        Status = PurchaseOrderStatus.Received;
    }

    public void Cancel()
    {
        if (Status == PurchaseOrderStatus.Received)
            throw new DomainExceptions.InvalidPurchaseOrderStatus(Status.ToString(), nameof(PurchaseOrderStatus.Cancelled));

        Status = PurchaseOrderStatus.Cancelled;
    }
}