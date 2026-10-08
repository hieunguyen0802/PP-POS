using Inventory.Application.DTOs.Inventory;
using MediatR;


namespace Inventory.Application.Features.InventoryItems.Queries.GetStockLevel;

public record GetStockLevelQuery(Guid ProductId) : IRequest<StockLevelDto>;