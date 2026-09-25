

using MediatR;
using POS.Domain.Exceptions;
using POS.Domain.Interfaces;

namespace POS.Application.Features.Orders.Commands.RemoveOrderItem;

public class RemoveOrderItemCommandHandler : IRequestHandler<RemoveOrderItemCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveOrderItemCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(RemoveOrderItemCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetWithDetailsAsync(request.OrderId, cancellationToken);
        if (order == null) throw new DomainExceptions.EntityNotFound("Order", request.OrderId);

        order.RemoveItem(request.ProductId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}