using Inventory.Domain.Exceptions;
using Inventory.Domain.Interfaces;
using Inventory.Domain.Entities;
using MediatR;

namespace Inventory.Application.Features.InventoryItems.Commands.AdjustStock;

public class AdjustStockCommandHandler : IRequestHandler<AdjustStockCommand, int>
{
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStockAdjustmentRepository _stockAdjustmentRepository;

    public AdjustStockCommandHandler(IInventoryItemRepository inventoryItemRepository, IUnitOfWork unitOfWork, IStockAdjustmentRepository stockAdjustmentRepository)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _unitOfWork = unitOfWork;
        _stockAdjustmentRepository = stockAdjustmentRepository;
    }

    public async Task<int> Handle(AdjustStockCommand request, CancellationToken cancellationToken)
    {
        var item = await _inventoryItemRepository.GetByProductIdAsync(request.ProductId, cancellationToken)
            ?? throw new DomainExceptions.EntityNotFound("InventoryItem", request.ProductId);

        item.Adjust(request.Delta);

        await _stockAdjustmentRepository.AddAsync(new StockAdjustment(request.ProductId, request.Delta, request.Reason), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return item.QuantityOnHand;
    }
}