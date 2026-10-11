using MediatR;

namespace Inventory.Application.Features.InventoryItems.Commands.ReceiveStock;

public record ReceiveStockCommand(Guid ProductId, int Quantity, string Reason) : IRequest<int>;