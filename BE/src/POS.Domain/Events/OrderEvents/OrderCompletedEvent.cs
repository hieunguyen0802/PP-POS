using POS.Domain.Common;
namespace POS.Domain.Events;

public class OrderCompletedEvent : IDomainEvent
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime OccurredOn { get; set; }

    public OrderCompletedEvent(Guid orderId, Guid customerId, decimal totalAmount, DateTime createdAt)
    {
        OrderId = orderId;
        CustomerId = customerId;
        TotalAmount = totalAmount;
        OccurredOn = createdAt;
    }
}