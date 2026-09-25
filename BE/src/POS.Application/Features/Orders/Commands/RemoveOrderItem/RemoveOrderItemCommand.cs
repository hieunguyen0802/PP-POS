using MediatR;

namespace POS.Application.Features.Orders.Commands.RemoveOrderItem;

public record RemoveOrderItemCommand(Guid OrderId, Guid ProductId) : IRequest<Unit>;