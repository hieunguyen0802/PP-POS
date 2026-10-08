using MediatR;
using POS.Application.Common.Interfaces;
using POS.Domain.Exceptions;
using POS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace POS.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInventoryService _inventoryService;
    private readonly ILogger<CancelOrderCommandHandler> _logger;

    public CancelOrderCommandHandler(
        IOrderRepository orderRepository,
        IInventoryService inventoryService,
        IUnitOfWork unitOfWork,
        ILogger<CancelOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _inventoryService = inventoryService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Unit> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetWithDetailsAsync(request.OrderId, cancellationToken);
        if (order == null)
        {
            throw new DomainExceptions.EntityNotFound("Order", request.OrderId);
        }
        if (order.OrderStatus == Domain.Enums.OrderStatus.Cancelled)
            return Unit.Value;

        order.Cancel();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Release only after the cancellation is saved
        foreach (var item in order.OrderItems)
        {
            try
            {
                await _inventoryService.ReleaseStockAsync(item.ProductId, item.Quantity, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Order {OrderId} cancelled but failed to release {Quantity} of product {ProductId}.",
                    order.Id, item.Quantity, item.ProductId);
            }
        }

        return Unit.Value;

    }
}