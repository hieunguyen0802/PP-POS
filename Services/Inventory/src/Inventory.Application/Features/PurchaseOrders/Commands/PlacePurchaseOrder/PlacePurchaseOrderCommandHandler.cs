using Inventory.Domain.Exceptions;
using Inventory.Domain.Interfaces;
using MediatR;

namespace Inventory.Application.Features.PurchaseOrders.Commands.PlacePurchaseOrder;

public class PlacePurchaseOrderCommandHandler : IRequestHandler<PlacePurchaseOrderCommand>
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PlacePurchaseOrderCommandHandler(IPurchaseOrderRepository purchaseOrderRepository, IUnitOfWork unitOfWork)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(PlacePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var po = await _purchaseOrderRepository.GetWithDetailsAsync(request.PurchaseOrderId, cancellationToken)
            ?? throw new DomainExceptions.EntityNotFound("PurchaseOrder", request.PurchaseOrderId);

        po.MarkAsOrdered();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}