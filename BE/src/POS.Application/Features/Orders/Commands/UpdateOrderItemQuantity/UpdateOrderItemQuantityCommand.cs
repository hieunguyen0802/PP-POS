

using MediatR;

namespace POS.Application.Features.Orders.Commands.UpdateOrderItemQuantity;

public record UpdateOrderItemQuantityCommand(Guid OrderId, Guid ProductId, int Quantity) : IRequest<Unit>;