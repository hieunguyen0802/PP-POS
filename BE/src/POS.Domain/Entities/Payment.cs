using POS.Domain.Common;
using POS.Domain.Events;
using POS.Domain.Helpers;

namespace POS.Domain.Entities;

public class Payment : AggregateRoot
{
    public string ReferenceNumber { get; private set; } = string.Empty;
    public decimal Amount { get; private set; } = 0m;
    public DateTime PaymentDate { get; private set; } = DateTime.Now;
    public PaymentMethod PaymentMethod { get; private set; }

    public Guid OrderId { get; private set; }
    public Order? Order { get; private set; }

    // For EF Core
    private Payment() { }
    public Payment(Guid orderId, PaymentMethod paymentMethod, decimal amount)
    {
        ValidatePayment(amount, orderId);
        OrderId = orderId;
        PaymentMethod = paymentMethod;
        Amount = amount;
        ReferenceNumber = CodeGenerator.GenerateReferenceNumber();
        AddDomainEvent(new PaymentProcessedEvent(Id, orderId, amount, paymentMethod, ReferenceNumber));
    }

    private void ValidatePayment(decimal amount, Guid orderId)
    {
        if (amount <= 0)
            throw new ArgumentException("Payment amount must be greater than zero.", nameof(amount));

        if (orderId == Guid.Empty)
            throw new ArgumentException("Order ID cannot be empty.", nameof(orderId));

    }
}