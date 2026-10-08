using MediatR;

namespace Inventory.Application.Features.InventoryItems.Commands.ReleaseStock;

public record ReleaseStockCommand(Guid ProductId, int Quantity) : IRequest<int>;