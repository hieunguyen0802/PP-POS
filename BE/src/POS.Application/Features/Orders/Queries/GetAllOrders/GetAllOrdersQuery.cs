using MediatR;
using POS.Application.DTOs.Order;

namespace POS.Application.Features.Orders.Queries.GetAllOrders;

public record GetAllOrdersQuery() : IRequest<List<OrderDto>>;
