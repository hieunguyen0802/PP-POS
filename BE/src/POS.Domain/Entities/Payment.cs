using POS.Domain.Common;
using POS.Domain.Events;

namespace POS.Domain.Entities;

public class Payment : AggregateRoot
{
    public string ReferenceNumber { get; private set; } = string.Empty;

    // What the customer handed over (can exceed the order balance for cash).
    public decimal AmountTendered { get; private set; }

    // What actually got applied to the order's balance (never exceeds RemainingBalance).
    public decimal AmountApplied { get; private set; }

    // Cash only: AmountTendered - AmountApplied. Zero for card/digital methods.
    public decimal ChangeGiven { get; private set; }

    public DateTime PaymentDate { get; private set; } = DateTime.UtcNow;
    public PaymentMethod PaymentMethod { get; private set; }

    public Guid OrderId { get; private set; }
    public Order? Order { get; private set; }

    // For EF Core
    private Payment() { }

    // Internal: only Order (the aggregate that owns the payment invariants -
    // overpayment/change rules) is allowed to construct a Payment. See Order.RecordPayment.
    internal Payment(Guid orderId, PaymentMethod paymentMethod, decimal amountTendered, decimal amountApplied, decimal changeGiven, string referenceNumber)
    {
        ValidatePayment(orderId, amountTendered, amountApplied, referenceNumber);

        OrderId = orderId;
        PaymentMethod = paymentMethod;
        AmountTendered = amountTendered;
        AmountApplied = amountApplied;
        ChangeGiven = changeGiven;
        ReferenceNumber = referenceNumber;

        AddDomainEvent(new PaymentProcessedEvent(Id, orderId, amountApplied, paymentMethod, ReferenceNumber));
    }

    private static void ValidatePayment(Guid orderId, decimal amountTendered, decimal amountApplied, string referenceNumber)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException("Order ID cannot be empty.", nameof(orderId));

        if (amountTendered <= 0)
            throw new ArgumentException("Amount tendered must be greater than zero.", nameof(amountTendered));

        if (amountApplied <= 0)
            throw new ArgumentException("Amount applied must be greater than zero.", nameof(amountApplied));

        if (string.IsNullOrWhiteSpace(referenceNumber))
            throw new ArgumentException("Reference number cannot be empty.", nameof(referenceNumber));
    }
}
