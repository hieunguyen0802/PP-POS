using POS.Domain.Common;
namespace POS.Domain.Events;

public class PaymentFailedEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public decimal AttemptedAmount { get; }
    public PaymentMethod PaymentMethod { get; }
    public string FailureReason { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public PaymentFailedEvent(
        Guid orderId,
        decimal attemptedAmount,
        PaymentMethod paymentMethod,
        string failureReason)
    {
        OrderId = orderId;
        AttemptedAmount = attemptedAmount;
        PaymentMethod = paymentMethod;
        FailureReason = failureReason;
    }
}