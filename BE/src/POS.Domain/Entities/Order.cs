using POS.Domain.Common;
using POS.Domain.Enums;
using POS.Domain.Helpers;
using POS.Domain.Exceptions;
using POS.Domain.Events;
using System.ComponentModel.DataAnnotations.Schema;
namespace POS.Domain.Entities;

public class Order : AggregateRoot
{
    //fields for Order
    public string OrderNumber { get; private set; } = string.Empty;
    public OrderStatus OrderStatus { get; private set; }
    public DateTime OrderDate { get; private set; } = DateTime.Today;
    public decimal TotalAmount { get; private set; } = 0m; // Initialize to 0.0
    public Guid CustomerId { get; private set; }
    public Customer? Customer { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [NotMapped]
    public decimal AmountPaid => Payments.Sum(p => p.AmountApplied);

    [NotMapped]
    public decimal RemainingBalance => TotalAmount - AmountPaid;

    [NotMapped]
    public bool IsFullyPaid => RemainingBalance <= 0;

    // empty constructor for EF Core
    private Order() { }


    // constructor for creating a new order
    public Order(Guid customerId, string orderNumber)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
            throw new ArgumentException("Order number cannot be empty.", nameof(orderNumber));

        Id = Guid.NewGuid();
        CustomerId = customerId;
        OrderNumber = orderNumber;
        OrderStatus = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        AddDomainEvent(new OrderCreatedEvent(Id, customerId, TotalAmount, CreatedAt));
    }


    // functions for adding, removing, and updating items in the order, as well as cancelling the order
    public void AddItem(Guid productId, int quantity, decimal price)
    {
        EnsureOrderIsEditable();
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        var orderItem = new OrderItem(productId, quantity, price);

        OrderItems.Add(orderItem);
        RecalculateTotalAmount();
    }

    public void RemoveItem(Guid productId)
    {
        EnsureOrderIsEditable();

        var item = OrderItems.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            OrderItems.Remove(item);
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

        var item = OrderItems.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
        {
            throw new DomainExceptions.EntityNotFound(nameof(OrderItem), productId);
        }

        item.UpdateQuantity(newQuantity);
        RecalculateTotalAmount();
    }

    public void Cancel()
    {
        if (OrderStatus == OrderStatus.Completed)
        {
            throw new DomainExceptions.InvalidOrderStatus(OrderStatus.ToString(), OrderStatus.Cancelled.ToString());
        }

        OrderStatus = OrderStatus.Cancelled;
        AddDomainEvent(new OrderCancelledEvent(Id, CustomerId, TotalAmount, DateTime.UtcNow));
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

    private void RecalculateTotalAmount()
    {
        TotalAmount = OrderItems.Sum(i => i.Quantity * i.UnitPrice);
    }

    // Records a payment against this order and applies the overpayment/change rules:
    // - Cash can be tendered over the remaining balance; the excess is returned as change.
    // - Card/digital methods must not exceed the remaining balance (no "change" concept for those).
    // Automatically transitions the order to Completed once it becomes fully paid.
    public Payment RecordPayment(PaymentMethod method, decimal amountTendered, string referenceNumber)
    {
        if (OrderStatus == OrderStatus.Cancelled)
            throw new DomainExceptions.InvalidOrderStatus(OrderStatus.ToString(), OrderStatus.Completed.ToString());

        if (amountTendered <= 0)
            throw new ArgumentException("Amount tendered must be greater than zero.", nameof(amountTendered));

        if (IsFullyPaid)
            throw new InvalidOperationException("Order is already fully paid.");

        var remaining = RemainingBalance;
        var amountApplied = Math.Min(amountTendered, remaining);

        if (method != PaymentMethod.Cash && amountTendered > remaining)
        {
            throw new InvalidOperationException(
                $"{method} payment ({amountTendered:C}) cannot exceed the remaining balance ({remaining:C}). Only cash payments may be tendered over the amount owed.");
        }

        var changeGiven = method == PaymentMethod.Cash ? amountTendered - amountApplied : 0m;

        if (referenceNumber == null)
            throw new ArgumentException("Reference number cannot be null.", nameof(referenceNumber));

        var payment = new Payment(Id, method, amountTendered, amountApplied, changeGiven, referenceNumber);
        Payments.Add(payment);
        UpdatedAt = DateTime.UtcNow;

        if (IsFullyPaid)
        {
            OrderStatus = OrderStatus.Completed;
            AddDomainEvent(new OrderCompletedEvent(Id, CustomerId, TotalAmount, UpdatedAt.Value));
        }

        return payment;
    }


}