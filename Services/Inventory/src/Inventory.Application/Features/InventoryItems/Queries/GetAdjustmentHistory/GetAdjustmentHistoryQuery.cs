using Inventory.Application.DTOs;
using MediatR;

namespace Inventory.Application.Features.InventoryItems.Queries.GetAdjustmentHistory;

public record GetAdjustmentHistoryQuery(Guid ProductId) : IRequest<IReadOnlyList<StockAdjustmentDto>>;