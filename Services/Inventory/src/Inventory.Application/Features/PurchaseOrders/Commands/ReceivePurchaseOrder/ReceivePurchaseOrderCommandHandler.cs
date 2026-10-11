using Inventory.Domain.Entities;
using Inventory.Domain.Exceptions;
using Inventory.Domain.Interfaces;
using MediatR;

namespace Inventory.Application.Features.PurchaseOrders.Commands.ReceivePurchaseOrder;

public class ReceivePurchaseOrderCommandHandler : IRequestHandler<ReceivePurchaseOrderCommand>
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IStockAdjustmentRepository _stockAdjustmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReceivePurchaseOrderCommandHandler(
        IPurchaseOrderRepository purchaseOrderRepository,
        IInventoryItemRepository inventoryItemRepository,
        IStockAdjustmentRepository stockAdjustmentRepository,
        IUnitOfWork unitOfWork)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _inventoryItemRepository = inventoryItemRepository;
        _stockAdjustmentRepository = stockAdjustmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ReceivePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var po = await _purchaseOrderRepository.GetWithDetailsAsync(request.PurchaseOrderId, cancellationToken)
            ?? throw new DomainExceptions.EntityNotFound("PurchaseOrder", request.PurchaseOrderId);

        po.MarkAsReceived();

        foreach (var line in po.Items)
        {
            var item = await _inventoryItemRepository.GetByProductIdAsync(line.ProductId, cancellationToken)
                ?? throw new DomainExceptions.EntityNotFound("InventoryItem", line.ProductId);

            item.Receive(line.Quantity);

            await _stockAdjustmentRepository.AddAsync(
                new StockAdjustment(line.ProductId, line.Quantity, $"Received on {po.OrderNumber}"),
                cancellationToken);
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}