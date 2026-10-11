using AutoMapper;
using Inventory.Application.DTOs;
using Inventory.Domain.Exceptions;
using Inventory.Domain.Interfaces;
using MediatR;

namespace Inventory.Application.Features.InventoryItems.Queries.GetAdjustmentHistory;

public class GetAdjustmentHistoryQueryHandler
    : IRequestHandler<GetAdjustmentHistoryQuery, IReadOnlyList<StockAdjustmentDto>>
{
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IStockAdjustmentRepository _stockAdjustmentRepository;
    private readonly IMapper _mapper;

    public GetAdjustmentHistoryQueryHandler(
        IInventoryItemRepository inventoryItemRepository,
        IStockAdjustmentRepository stockAdjustmentRepository,
        IMapper mapper)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _stockAdjustmentRepository = stockAdjustmentRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<StockAdjustmentDto>> Handle(
        GetAdjustmentHistoryQuery request, CancellationToken cancellationToken)
    {
        // Unknown product => 404, instead of an empty list that hides a typo'd id
        if (!await _inventoryItemRepository.ExistsByProductIdAsync(request.ProductId, cancellationToken))
            throw new DomainExceptions.EntityNotFound("InventoryItem", request.ProductId);

        var adjustments = await _stockAdjustmentRepository.GetByProductIdAsync(request.ProductId, cancellationToken);
        return _mapper.Map<IReadOnlyList<StockAdjustmentDto>>(adjustments);
    }
}