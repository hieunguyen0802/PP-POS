
using Microsoft.Extensions.Logging;
using MediatR;
using POS.Domain.Exceptions;
using POS.Domain.Interfaces;
using POS.Application.Common.Interfaces;

namespace POS.Application.Features.Orders.Commands.UpdateOrderItemQuantity;

public class UpdateOrderItemQuantityCommandHandler : IRequestHandler<UpdateOrderItemQuantityCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInventoryService _inventoryService;
    private readonly ILogger<UpdateOrderItemQuantityCommandHandler> _logger;
    private readonly IProductRepository _productRepository;

    public UpdateOrderItemQuantityCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IInventoryService inventoryService,
        ILogger<UpdateOrderItemQuantityCommandHandler> logger, IProductRepository productRepository
    )
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _inventoryService = inventoryService;
        _logger = logger;
        _productRepository = productRepository;
    }

    public async Task<Unit> Handle(UpdateOrderItemQuantityCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetWithDetailsAsync(request.OrderId, cancellationToken);
        if (order == null) throw new DomainExceptions.EntityNotFound("Order", request.OrderId);


        var item = order.OrderItems.FirstOrDefault(i => i.ProductId == request.ProductId);
        var oldQuantity = item?.Quantity ?? 0;
        order.UpdateItemQuantity(request.ProductId, request.Quantity);

        var difference = request.Quantity - oldQuantity;

        if (difference > 0)
        {
            // Taking more stock: reserve first, save second, undo the reservation if the save fails
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            var productName = product?.ProductName ?? request.ProductId.ToString();

            var reservation = await _inventoryService.ReserveStockAsync(request.ProductId, difference, cancellationToken);
            if (!reservation.Success)
                throw new DomainExceptions.InsufficientStock(productName, difference);

            try
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                await TryReleaseAsync(order.Id, request.ProductId, difference);
                throw;
            }
        }
        else
        {
            // Giving stock back (or no change): save first, release second
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (difference < 0)
                await TryReleaseAsync(order.Id, request.ProductId, -difference);
        }
        return Unit.Value;
    }

    private async Task TryReleaseAsync(Guid orderId, Guid productId, int quantity)
    {
        try
        {
            await _inventoryService.ReleaseStockAsync(productId, quantity, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to release {Quantity} of product {ProductId} for order {OrderId}.",
                quantity, productId, orderId);
        }
    }
}