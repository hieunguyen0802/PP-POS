using MediatR;

namespace Inventory.Application.Features.InventoryItems.Commands.CreateInventoryItem;

public record CreateInventoryItemCommand(Guid ProductId, int InitialQuantity, int ReorderThreshold) : IRequest<Guid>;