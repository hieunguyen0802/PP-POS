using MediatR;

namespace Inventory.Application.Features.PurchaseOrders.Commands.CancelPurchaseOrder;

public record CancelPurchaseOrderCommand(Guid PurchaseOrderId) : IRequest;