namespace Inventory.Application.DTOs;

public record StockLevelDto
(Guid ProductId, int Quantity, int ReorderLevel, bool IsBelowReorderLevel);
