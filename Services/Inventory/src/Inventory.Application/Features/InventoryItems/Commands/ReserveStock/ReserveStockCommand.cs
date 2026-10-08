using MediatR;

namespace Inventory.Application.Features.InventoryItems.Commands.ReserveStock;

// Returns the quantity on hand remaining after the reservation.
public record ReserveStockCommand(Guid ProductId, int Quantity) : IRequest<int>;