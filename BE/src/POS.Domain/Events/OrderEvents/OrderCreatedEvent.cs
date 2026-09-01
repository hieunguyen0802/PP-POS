public class OrderCompletedEvent : IDomainEvent
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime OccurredOn { get; set; }

    public OrderCompletedEvent(Guid orderId, Guid customerId, decimal totalAmount, DateTime completedAt)
    {
        OrderId = orderId;
        CustomerId = customerId;
        TotalAmount = totalAmount;
        OccurredOn = completedAt;
    }
}