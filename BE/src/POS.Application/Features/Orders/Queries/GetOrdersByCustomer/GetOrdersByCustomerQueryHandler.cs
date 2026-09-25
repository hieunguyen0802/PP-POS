using AutoMapper;
using MediatR;
using POS.Application.DTOs.Order;
using POS.Domain.Exceptions;
using POS.Domain.Interfaces;

namespace POS.Application.Features.Orders.Queries.GetOrdersByCustomer;

public class GetOrdersByCustomerQueryHandler : IRequestHandler<GetOrdersByCustomerQuery, List<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetOrdersByCustomerQueryHandler(
        IOrderRepository orderRepository, IMapper mapper
        )
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<List<OrderDto>> Handle(GetOrdersByCustomerQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetOrdersByCustomerAsync(request.CustomerId, cancellationToken);
        return _mapper.Map<List<OrderDto>>(order);
    }
}
