using MediatR;
using POS.Application.DTOs.Order;

namespace POS.Application.Features.Orders.Commands.CreateOrder;

public record CreateOrderCommand(
    Guid CustomerId,
    List<CreateOrderItemDto> Items
) : IRequest<Guid>;