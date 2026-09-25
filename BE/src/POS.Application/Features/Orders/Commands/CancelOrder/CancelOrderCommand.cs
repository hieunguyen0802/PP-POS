using MediatR;
using POS.Application.DTOs.Order;

namespace POS.Application.Features.Orders.Commands.CancelOrder;

public record CancelOrderCommand(Guid OrderId) : IRequest<Unit>;