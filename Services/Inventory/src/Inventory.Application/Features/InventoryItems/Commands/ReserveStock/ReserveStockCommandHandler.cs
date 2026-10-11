using Inventory.Domain.Exceptions;
using Inventory.Domain.Interfaces;
using MediatR;

namespace Inventory.Application.Features.InventoryItems.Commands.ReserveStock;

public class ReserveStockCommandHandler : IRequestHandler<ReserveStockCommand, int>
{
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReserveStockCommandHandler(IInventoryItemRepository inventoryItemRepository, IUnitOfWork unitOfWork)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(ReserveStockCommand request, CancellationToken cancellationToken)
    {
        var inventoryItem = await _inventoryItemRepository.GetByProductIdAsync(request.ProductId, cancellationToken);

        // No stock row for this product means nothing is available: refuse the sale.
        if (inventoryItem == null)
            throw new DomainExceptions.InsufficientStock(request.ProductId, request.Quantity, 0);

        inventoryItem.Reserve(request.Quantity);

        // No Update() call needed: EF is already tracking this entity and will save the change.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return inventoryItem.QuantityOnHand;
    }
}