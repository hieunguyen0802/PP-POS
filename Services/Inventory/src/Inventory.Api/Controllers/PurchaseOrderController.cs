
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Inventory.Application.Features.PurchaseOrders.Queries.GetPurchaseOrder;
using Inventory.Application.Features.PurchaseOrders.Commands.CreatePurchaseOrder;
using Inventory.Application.Features.PurchaseOrders.Commands.PlacePurchaseOrder;
using Inventory.Application.Features.PurchaseOrders.Commands.ReceivePurchaseOrder;
using Inventory.Application.Features.PurchaseOrders.Commands.CancelPurchaseOrder;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize(Policy = "StockManager")]
public class PurchaseOrderController : ControllerBase
{
    private readonly IMediator _mediator;
    public PurchaseOrderController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderCommand command, CancellationToken ct)
        => Ok(await _mediator.Send(command, ct));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPurchaseOrder(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetPurchaseOrderQuery(id), ct));

    [HttpPost("{id}")]
    public async Task<IActionResult> PlacePurchaseOrder(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new PlacePurchaseOrderCommand(id), ct);
        return NoContent();
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> ReceivePurchaseOrder(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new ReceivePurchaseOrderCommand(id), ct);
        return NoContent();
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> CancelPurchaseOrder(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new CancelPurchaseOrderCommand(id), ct);
        return NoContent();
    }
}