
public class Order : EntityBase
{
    public string OrderNumber { get; set; } = string.Empty;
    public OrderStatus OrderStatus { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Today;
    public decimal TotalAmount { get; set; } = 0m; // Initialize to 0.0
    public Guid CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}