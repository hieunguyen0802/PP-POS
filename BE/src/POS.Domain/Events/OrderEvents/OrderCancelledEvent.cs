public class OrderCancelledEvent : IDomainEvent
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime OccurredOn { get; set; }

    public OrderCancelledEvent(Guid orderId, Guid customerId, decimal totalAmount, DateTime cancelledAt)
    {
        OrderId = orderId;
        CustomerId = customerId;
        TotalAmount = totalAmount;
        OccurredOn = cancelledAt;
    }
}