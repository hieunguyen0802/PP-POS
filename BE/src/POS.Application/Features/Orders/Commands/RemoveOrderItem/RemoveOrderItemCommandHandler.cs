
using Microsoft.Extensions.Logging;
using MediatR;
using POS.Domain.Exceptions;
using POS.Domain.Interfaces;
using POS.Application.Common.Interfaces;

namespace POS.Application.Features.Orders.Commands.RemoveOrderItem;

public class RemoveOrderItemCommandHandler : IRequestHandler<RemoveOrderItemCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInventoryService _inventoryService;
    private readonly ILogger<RemoveOrderItemCommandHandler> _logger;

    public RemoveOrderItemCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IInventoryService inventoryService, ILogger<RemoveOrderItemCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _inventoryService = inventoryService;
        _logger = logger;
    }

    public async Task<Unit> Handle(RemoveOrderItemCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetWithDetailsAsync(request.OrderId, cancellationToken);
        if (order == null) throw new DomainExceptions.EntityNotFound("Order", request.OrderId);


        var item = order.OrderItems.FirstOrDefault(i => i.ProductId == request.ProductId);
        var quantityToRelease = item?.Quantity ?? 0;

        order.RemoveItem(request.ProductId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (quantityToRelease > 0)
        {
            try
            {
                await _inventoryService.ReleaseStockAsync(request.ProductId, quantityToRelease, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Item removed from order {OrderId} but failed to release {Quantity} of product {ProductId}.",
                    order.Id, quantityToRelease, request.ProductId);
            }
        }


        return Unit.Value;
    }
}