using MediatR;
using POS.Application.DTOs.Order;

namespace POS.Application.Features.Orders.Queries.GetOrderById;

public record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDto>;