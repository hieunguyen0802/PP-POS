using Inventory.Domain.Exceptions;
using Inventory.Domain.Interfaces;
using Inventory.Domain.Entities;
using MediatR;

namespace Inventory.Application.Features.InventoryItems.Commands.ReceiveStock;

public class ReceiveStockCommandHandler : IRequestHandler<ReceiveStockCommand, int>
{
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStockAdjustmentRepository _stockAdjustmentRepository;

    public ReceiveStockCommandHandler(IInventoryItemRepository inventoryItemRepository, IUnitOfWork unitOfWork, IStockAdjustmentRepository stockAdjustmentRepository)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _unitOfWork = unitOfWork;
        _stockAdjustmentRepository = stockAdjustmentRepository;
    }

    public async Task<int> Handle(ReceiveStockCommand request, CancellationToken cancellationToken)
    {
        var item = await _inventoryItemRepository.GetByProductIdAsync(request.ProductId, cancellationToken)
            ?? throw new DomainExceptions.EntityNotFound("InventoryItem", request.ProductId);

        item.Receive(request.Quantity);

        await _stockAdjustmentRepository.AddAsync(new StockAdjustment(request.ProductId, request.Quantity, request.Reason), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return item.QuantityOnHand;
    }
}