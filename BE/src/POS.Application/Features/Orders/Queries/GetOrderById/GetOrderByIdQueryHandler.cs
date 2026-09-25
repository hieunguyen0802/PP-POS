using AutoMapper;
using MediatR;
using POS.Application.DTOs.Order;
using POS.Domain.Exceptions;
using POS.Domain.Interfaces;

namespace POS.Application.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetOrderByIdQueryHandler(
        IOrderRepository orderRepository, IMapper mapper
        )
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetWithDetailsAsync(request.OrderId, cancellationToken);
        if (order == null)
        {
            throw new DomainExceptions.EntityNotFound("Order", request.OrderId);
        }

        return _mapper.Map<OrderDto>(order);

    }
}