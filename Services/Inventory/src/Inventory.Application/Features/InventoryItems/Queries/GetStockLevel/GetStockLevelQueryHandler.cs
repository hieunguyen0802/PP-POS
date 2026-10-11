using AutoMapper;
using Inventory.Application.DTOs;
using Inventory.Domain.Interfaces;
using Inventory.Domain.Exceptions;
using MediatR;

namespace Inventory.Application.Features.InventoryItems.Queries.GetStockLevel;

public class GetStockLevelQueryHandler : IRequestHandler<GetStockLevelQuery, StockLevelDto>
{
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IMapper _mapper;

    public GetStockLevelQueryHandler(IInventoryItemRepository inventoryItemRepository, IMapper mapper)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _mapper = mapper;
    }

    public async Task<StockLevelDto> Handle(GetStockLevelQuery request, CancellationToken cancellationToken)
    {
        var inventoryItem = await _inventoryItemRepository.GetByProductIdAsync(request.ProductId, cancellationToken);

        if (inventoryItem == null)
        {
            throw new DomainExceptions.EntityNotFound("InventoryItem", request.ProductId);
        }

        return _mapper.Map<StockLevelDto>(inventoryItem);
    }
}
