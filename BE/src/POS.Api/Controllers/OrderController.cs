using MediatR;
using Microsoft.AspNetCore.Mvc;
using POS.Application.Features.Orders.Commands.CreateOrder;

namespace POS.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]

public class OrderController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var orderId = await _mediator.Send(command, cancellationToken);
        return Ok(orderId);
    }
}