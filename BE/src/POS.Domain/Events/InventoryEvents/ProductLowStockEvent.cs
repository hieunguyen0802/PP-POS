public class ProductLowStockEvent : IDomainEvent
{
    public Guid ProductId { get; set; }
    public int CurrentStock { get; set; }
    public DateTime OccurredOn { get; set; }

    public ProductLowStockEvent(Guid productId, int currentStock, DateTime occurredOn)
    {
        ProductId = productId;
        CurrentStock = currentStock;
        OccurredOn = occurredOn;
    }
}