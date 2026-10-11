namespace Inventory.Application.DTOs
{
    public record StockAdjustmentDto(int Quantity, string Reason, string CreatedBy, DateTime CreatedAt);
}