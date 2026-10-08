using Inventory.Domain.Exceptions;
using Inventory.Domain.Interfaces;
using MediatR;

namespace Inventory.Application.Features.InventoryItems.Commands.ReleaseStock;

public class ReleaseStockCommandHandler : IRequestHandler<ReleaseStockCommand, int>
{
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReleaseStockCommandHandler(IInventoryItemRepository inventoryItemRepository, IUnitOfWork unitOfWork)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(ReleaseStockCommand request, CancellationToken cancellationToken)
    {
        var item = await _inventoryItemRepository.GetByProductIdAsync(request.ProductId, cancellationToken)
            ?? throw new DomainExceptions.EntityNotFound("InventoryItem", request.ProductId);

        item.Release(request.Quantity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return item.QuantityOnHand;
    }
}