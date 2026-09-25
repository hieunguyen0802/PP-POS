using MediatR;
using POS.Domain.Exceptions;
using POS.Domain.Interfaces;

namespace POS.Application.Features.Orders.Commands.AddOrderItem;

public class AddOrderItemCommandHandler : IRequestHandler<AddOrderItemCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;

    public AddOrderItemCommandHandler(
        IOrderRepository orderRepository, IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(AddOrderItemCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetWithDetailsAsync(request.OrderId, cancellationToken);
        if (order == null)
        {
            throw new DomainExceptions.EntityNotFound("Order", request.OrderId);
        }

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
        {
            throw new DomainExceptions.EntityNotFound("Product", request.ProductId);
        }

        order.AddItem(product.Id, request.Quantity, product.UnitPrice);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;

    }
}