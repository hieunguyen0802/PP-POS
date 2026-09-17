using POS.Domain.Common;
using POS.Domain.Enums;
using POS.Domain.Helpers;
using POS.Domain.Exceptions;

namespace POS.Domain.Entities;

public class Order : EntityBase
{
    public string OrderNumber { get; set; } = string.Empty;
    public OrderStatus OrderStatus { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Today;
    public decimal TotalAmount { get; set; } = 0m; // Initialize to 0.0
    public Guid CustomerId { get; set; }
    public Customer? Customer { get; set; }
    private readonly List<OrderItem> _items = new();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    private Order() { }

    public Order(Guid customerId)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        OrderNumber = CodeGenerator.GenerateOrderNumber();
        OrderStatus = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddItem(Guid productId, int quantity, decimal price)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        var orderItem = new OrderItem
        {
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = price
        };

        OrderItems.Add(orderItem);
        TotalAmount += quantity * price; // Update total amount
    }

    public void RemoveItem(Guid productId)
    {
        EnsureOrderIsEditable();

        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            _items.Remove(item);
            RecalculateTotalAmount();
        }
    }

    public void UpdateItemQuantity(Guid productId, int newQuantity)
    {
        EnsureOrderIsEditable();

        if (newQuantity <= 0)
        {
            RemoveItem(productId);
            return;
        }

        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
        {
            throw new DomainExceptions.EntityNotFound(nameof(OrderItem), productId);
        }

        item.UpdateQuantity(newQuantity);
        RecalculateTotalAmount();
    }

    // 3. Cancel the order
    public void Cancel()
    {
        if (OrderStatus == OrderStatus.Completed)
        {
            throw new DomainExceptions.InvalidOrderStatus(OrderStatus.ToString(), OrderStatus.Cancelled.ToString());
        }

        OrderStatus = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    // Guard method to enforce state rules
    private void EnsureOrderIsEditable()
    {
        if (OrderStatus != OrderStatus.Pending)
        {
            throw new InvalidOperationException("Cannot modify an order that is already completed or cancelled.");
        }
    }


}