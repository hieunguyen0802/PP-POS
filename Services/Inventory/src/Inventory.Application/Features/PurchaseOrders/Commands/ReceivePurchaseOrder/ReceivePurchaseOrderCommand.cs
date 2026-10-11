using MediatR;

namespace Inventory.Application.Features.PurchaseOrders.Commands.ReceivePurchaseOrder;

public record ReceivePurchaseOrderCommand(Guid PurchaseOrderId) : IRequest;