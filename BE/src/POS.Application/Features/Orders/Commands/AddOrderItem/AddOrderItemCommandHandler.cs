using MediatR;
using POS.Application.Common.Interfaces;
using POS.Domain.Exceptions;
using POS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace POS.Application.Features.Orders.Commands.AddOrderItem;

public class AddOrderItemCommandHandler : IRequestHandler<AddOrderItemCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;
    private readonly IInventoryService _inventoryService;
    private readonly ILogger<AddOrderItemCommandHandler> _logger;

    public AddOrderItemCommandHandler(
        IOrderRepository orderRepository, IProductRepository productRepository,
        IInventoryService inventoryService, ILogger<AddOrderItemCommandHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _inventoryService = inventoryService;
        _logger = logger;
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
        // 1. Change the order in memory first: throws if the order isn't editable
        order.AddItem(product.Id, request.Quantity, product.UnitPrice);


        // 2. Take the stock
        var reservation = await _inventoryService.ReserveStockAsync(product.Id, request.Quantity, cancellationToken);
        if (!reservation.Success)
            throw new DomainExceptions.InsufficientStock(product.ProductName, request.Quantity);


        // 3. Save. If that fails, give the stock back
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            try
            {
                await _inventoryService.ReleaseStockAsync(product.Id, request.Quantity, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to release {Quantity} of product {ProductId} after a failed add-item on order {OrderId}.",
                    request.Quantity, product.Id, order.Id);
            }
            throw;
        }

        return Unit.Value;

    }
}