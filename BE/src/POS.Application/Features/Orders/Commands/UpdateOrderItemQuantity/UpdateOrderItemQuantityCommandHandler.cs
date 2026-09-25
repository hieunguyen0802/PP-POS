

using MediatR;
using POS.Domain.Exceptions;
using POS.Domain.Interfaces;

namespace POS.Application.Features.Orders.Commands.UpdateOrderItemQuantity;

public class UpdateOrderItemQuantityCommandHandler : IRequestHandler<UpdateOrderItemQuantityCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrderItemQuantityCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateOrderItemQuantityCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetWithDetailsAsync(request.OrderId, cancellationToken);
        if (order == null) throw new DomainExceptions.EntityNotFound("Order", request.OrderId);

        order.UpdateItemQuantity(request.ProductId, request.Quantity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}