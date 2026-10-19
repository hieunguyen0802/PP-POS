using Inventory.Application.Features.InventoryItems.Queries.GetStockLevel;
using Inventory.Application.Features.InventoryItems.Commands.CreateInventoryItem;
using Inventory.Application.Features.InventoryItems.Commands.ReserveStock;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Inventory.Application.Features.InventoryItems.Commands.ReceiveStock;
using Inventory.Application.Features.InventoryItems.Commands.AdjustStock;
using Inventory.Application.Features.InventoryItems.Queries.GetAdjustmentHistory;

namespace Inventory.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = "StockReader")]
    [HttpGet("{productId}")]
    public async Task<IActionResult> GetStockLevel(Guid productId, CancellationToken cancellationToken)
    {
        var stockLevel = await _mediator.Send(new GetStockLevelQuery(productId), cancellationToken);
        return Ok(stockLevel);
    }


    [Authorize(Policy = "StockManager")]
    [HttpPost]
    public async Task<IActionResult> CreateInventoryItem([FromBody] CreateInventoryItemCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return Ok(id);
    }

    [Authorize(Policy = "InternalService")]
    [HttpPost]
    public async Task<IActionResult> ReserveStock([FromBody] ReserveStockCommand command, CancellationToken cancellationToken)
    {
        var remaining = await _mediator.Send(command, cancellationToken);
        return Ok(remaining);
    }

    [Authorize(Policy = "StockManager")]
    [HttpPost]
    public async Task<IActionResult> ReceiveStock([FromBody] ReceiveStockCommand command, CancellationToken cancellationToken)
    {
        var newQuantity = await _mediator.Send(command, cancellationToken);
        return Ok(newQuantity);
    }

    [Authorize(Policy = "StockManager")]
    [HttpPost]
    public async Task<IActionResult> AdjustStock([FromBody] AdjustStockCommand command, CancellationToken cancellationToken)
    {
        var newQuantity = await _mediator.Send(command, cancellationToken);
        return Ok(newQuantity);
    }

    [Authorize(Policy = "StockReader")]
    [HttpGet("{productId}")]
    public async Task<IActionResult> GetAdjustmentHistory(Guid productId, CancellationToken cancellationToken)
    {
        var history = await _mediator.Send(new GetAdjustmentHistoryQuery(productId), cancellationToken);
        return Ok(history);
    }


}