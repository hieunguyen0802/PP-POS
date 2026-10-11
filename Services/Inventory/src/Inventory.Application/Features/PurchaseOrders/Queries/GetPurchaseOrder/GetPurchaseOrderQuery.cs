using Inventory.Application.DTOs;
using MediatR;


namespace Inventory.Application.Features.PurchaseOrders.Queries.GetPurchaseOrder;

public record GetPurchaseOrderQuery(Guid ProductId) : IRequest<PurchaseOrderDto>;