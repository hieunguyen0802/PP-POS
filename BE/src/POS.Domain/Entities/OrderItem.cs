using POS.Domain.Common;
namespace POS.Domain.Entities;

public class OrderItem : EntityBase
{
    public decimal UnitPrice { get; set; } = 0m;
    public int Quantity { get; set; } = 1;
    public decimal TotalPrice => Quantity * UnitPrice;

    public Guid OrderId { get; set; }
    public Order? Order { get; set; }
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }

}