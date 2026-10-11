using AutoMapper;
using Inventory.Application.DTOs;
using Inventory.Domain.Interfaces;
using Inventory.Domain.Exceptions;
using MediatR;

namespace Inventory.Application.Features.PurchaseOrders.Queries.GetPurchaseOrder;

public class GetPurchaseOrderQueryHandler : IRequestHandler<GetPurchaseOrderQuery, PurchaseOrderDto>
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly IMapper _mapper;

    public GetPurchaseOrderQueryHandler(IPurchaseOrderRepository purchaseOrderRepository, IMapper mapper)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _mapper = mapper;
    }

    public async Task<PurchaseOrderDto> Handle(GetPurchaseOrderQuery request, CancellationToken cancellationToken)
    {
        var purchaseOrder = await _purchaseOrderRepository.GetWithDetailsAsync(request.ProductId, cancellationToken);

        if (purchaseOrder == null)
        {
            throw new DomainExceptions.EntityNotFound("PurchaseOrder", request.ProductId);
        }

        return _mapper.Map<PurchaseOrderDto>(purchaseOrder);
    }
}
