
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Inventory.Application.Features.Suppliers.Commands.CreateSupplier;
using Inventory.Application.Features.Suppliers.Queries.GetSuppliers;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize(Policy = "StockManager")]
public class SuppliersController : ControllerBase
{
    private readonly IMediator _mediator;
    public SuppliersController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSupplierCommand command, CancellationToken ct)
        => Ok(await _mediator.Send(command, ct));

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _mediator.Send(new GetSuppliersQuery(), ct));
}