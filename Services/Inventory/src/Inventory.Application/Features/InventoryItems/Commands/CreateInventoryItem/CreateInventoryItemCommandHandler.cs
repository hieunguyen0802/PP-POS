
using Inventory.Domain.Entities;
using Inventory.Domain.Exceptions;
using Inventory.Domain.Interfaces;
using MediatR;

namespace Inventory.Application.Features.InventoryItems.Commands.CreateInventoryItem;

public class CreateInventoryItemCommandHandler : IRequestHandler<CreateInventoryItemCommand, Guid>
{
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInventoryItemCommandHandler(IInventoryItemRepository inventoryItemRepository, IUnitOfWork unitOfWork)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateInventoryItemCommand request, CancellationToken cancellationToken)
    {
        // TODO: confirm the product really exists in POS (gRPC/REST call) before creating a stock row.
        // There is no foreign key to catch a bogus ProductId, so this check has to live in code.

        if (await _inventoryItemRepository.ExistsByProductIdAsync(request.ProductId, cancellationToken))
        {
            throw new DomainExceptions.EntityAlreadyExists("InventoryItem", request.ProductId);
        }

        var inventoryItem = new InventoryItem(request.ProductId, request.InitialQuantity, request.ReorderThreshold);

        await _inventoryItemRepository.AddAsync(inventoryItem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return inventoryItem.Id;
    }
}