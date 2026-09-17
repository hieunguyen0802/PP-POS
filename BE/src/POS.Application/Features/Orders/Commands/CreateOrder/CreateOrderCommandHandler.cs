using MediatR;
using POS.Domain.Common;
using POS.Domain.Exceptions;
using POS.Domain.Interfaces;
using POS.Domain.Entities;

namespace POS.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order(request.CustomerId);

        foreach (var itemDto in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(itemDto.ProductId, cancellationToken);
            if (product == null)
            {
                throw new DomainExceptions.EntityNotFound("Product", itemDto.ProductId);
            }
            _productRepository.Update(product);
            order.AddItem(product.Id, itemDto.Quantity, product.UnitPrice);
        }

        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}