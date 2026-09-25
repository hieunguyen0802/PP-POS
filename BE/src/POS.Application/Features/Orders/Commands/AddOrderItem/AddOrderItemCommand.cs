using MediatR;
using POS.Application.DTOs.Order;

namespace POS.Application.Features.Orders.Commands.AddOrderItem;

public record AddOrderItemCommand(Guid OrderId, Guid ProductId, int Quantity) : IRequest<Unit>;