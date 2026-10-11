using MediatR;

namespace Inventory.Application.Features.PurchaseOrders.Commands.PlacePurchaseOrder;

public record PlacePurchaseOrderCommand(Guid PurchaseOrderId) : IRequest;