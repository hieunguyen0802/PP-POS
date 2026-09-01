public class OrderCreatedEvent : IDomainEvent
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime OccurredOn { get; set; }

    public OrderCreatedEvent(Guid orderId, Guid customerId, decimal totalAmount, DateTime createdAt)
    {
        OrderId = orderId;
        CustomerId = customerId;
        TotalAmount = totalAmount;
        OccurredOn = createdAt;
    }
}