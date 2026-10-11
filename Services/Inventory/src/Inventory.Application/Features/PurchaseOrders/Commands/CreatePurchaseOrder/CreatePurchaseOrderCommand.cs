using MediatR;

namespace Inventory.Application.Features.PurchaseOrders.Commands.CreatePurchaseOrder;

public record PurchaseOrderLine(Guid ProductId, int Quantity, decimal UnitPrice);

public record CreatePurchaseOrderCommand(Guid SupplierId, IReadOnlyList<PurchaseOrderLine> Items) : IRequest<Guid>;