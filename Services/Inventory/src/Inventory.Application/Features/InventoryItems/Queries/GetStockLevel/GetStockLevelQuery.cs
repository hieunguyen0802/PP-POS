using Inventory.Application.DTOs;
using MediatR;


namespace Inventory.Application.Features.InventoryItems.Queries.GetStockLevel;

public record GetStockLevelQuery(Guid ProductId) : IRequest<StockLevelDto>;