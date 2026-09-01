public class PaymentProcessedEvent : IDomainEvent
{
    public Guid PaymentId { get; }
    public Guid OrderId { get; }
    public decimal Amount { get; }
    public PaymentMethod PaymentMethod { get; }
    public string ReferenceNumber { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public PaymentProcessedEvent(
        Guid paymentId,
        Guid orderId,
        decimal amount,
        PaymentMethod paymentMethod,
        string referenceNumber)
    {
        PaymentId = paymentId;
        OrderId = orderId;
        Amount = amount;
        PaymentMethod = paymentMethod;
        ReferenceNumber = referenceNumber;
    }
}