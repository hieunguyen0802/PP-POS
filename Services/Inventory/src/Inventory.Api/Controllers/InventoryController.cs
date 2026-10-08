using Inventory.Application.Features.InventoryItems.Queries.GetStockLevel;
using Inventory.Application.Features.InventoryItems.Commands.CreateInventoryItem;
using Inventory.Application.Features.InventoryItems.Commands.ReserveStock;
using Microsoft.AspNetCore.Mvc;
using MediatR;

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

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetStockLevel(Guid productId, CancellationToken cancellationToken)
    {
        var stockLevel = await _mediator.Send(new GetStockLevelQuery(productId), cancellationToken);
        return Ok(stockLevel);
    }

    [HttpPost]
    public async Task<IActionResult> CreateInventoryItem([FromBody] CreateInventoryItemCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return Ok(id);
    }

    [HttpPost]
    public async Task<IActionResult> ReserveStock([FromBody] ReserveStockCommand command, CancellationToken cancellationToken)
    {
        var remaining = await _mediator.Send(command, cancellationToken);
        return Ok(remaining);
    }
}