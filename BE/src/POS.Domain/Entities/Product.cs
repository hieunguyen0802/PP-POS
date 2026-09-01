public class Product : EntityBase
{
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; } = 0m;
    public int QuantityInStock { get; set; } = 0;
}