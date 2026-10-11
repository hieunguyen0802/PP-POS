using MediatR;

namespace Inventory.Application.Features.InventoryItems.Commands.AdjustStock;

public record AdjustStockCommand(Guid ProductId, int Delta, string Reason) : IRequest<int>;