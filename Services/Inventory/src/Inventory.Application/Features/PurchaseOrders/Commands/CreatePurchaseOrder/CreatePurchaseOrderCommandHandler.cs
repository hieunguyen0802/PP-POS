using Inventory.Domain.Entities;
using Inventory.Domain.Exceptions;
using Inventory.Domain.Interfaces;
using MediatR;

namespace Inventory.Application.Features.PurchaseOrders.Commands.CreatePurchaseOrder;

public class CreatePurchaseOrderCommandHandler : IRequestHandler<CreatePurchaseOrderCommand, Guid>
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePurchaseOrderCommandHandler(
        IPurchaseOrderRepository purchaseOrderRepository,
        ISupplierRepository supplierRepository,
        IInventoryItemRepository inventoryItemRepository,
        IUnitOfWork unitOfWork)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _supplierRepository = supplierRepository;
        _inventoryItemRepository = inventoryItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        // this writing style equals to have the supplier and null-check on supplier.
        _ = await _supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken)
            ?? throw new DomainExceptions.EntityNotFound("Supplier", request.SupplierId);



        foreach (var line in request.Items)
        {
            if (!await _inventoryItemRepository.ExistsByProductIdAsync(line.ProductId, cancellationToken))
                throw new DomainExceptions.EntityNotFound("InventoryItem", line.ProductId);
        }

        var orderNumber = $"PO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
        var purchaseOrder = new PurchaseOrder(orderNumber, DateTime.UtcNow, request.SupplierId);

        foreach (var line in request.Items)
            purchaseOrder.AddItem(line.ProductId, line.Quantity, line.UnitPrice);

        await _purchaseOrderRepository.AddAsync(purchaseOrder, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return purchaseOrder.Id;
    }
}