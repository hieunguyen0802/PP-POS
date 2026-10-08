namespace Inventory.Application.DTOs.Inventory;

public record StockLevelDto
(Guid ProductId, int Quantity, int ReorderLevel, bool IsBelowReorderLevel);
