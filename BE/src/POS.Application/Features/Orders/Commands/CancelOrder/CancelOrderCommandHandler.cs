using MediatR;
using POS.Domain.Exceptions;
using POS.Domain.Interfaces;

namespace POS.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelOrderCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null)
        {
            throw new DomainExceptions.EntityNotFound("Order", request.OrderId);
        }

        order.Cancel();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;

    }
}