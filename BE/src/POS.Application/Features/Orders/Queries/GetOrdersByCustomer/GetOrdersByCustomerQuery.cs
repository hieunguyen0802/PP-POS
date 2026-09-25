using MediatR;
using POS.Application.DTOs.Order;

namespace POS.Application.Features.Orders.Queries.GetOrdersByCustomer;

public record GetOrdersByCustomerQuery(Guid CustomerId) : IRequest<List<OrderDto>>;