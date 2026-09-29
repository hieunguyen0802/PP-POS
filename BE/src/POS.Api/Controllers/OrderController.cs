using MediatR;
using Microsoft.AspNetCore.Mvc;
using POS.Application.Features.Orders.Commands.CreateOrder;
using POS.Application.Features.Orders.Commands.CancelOrder;
using POS.Application.Features.Orders.Commands.AddOrderItem;
using POS.Application.Features.Orders.Commands.RemoveOrderItem;
using POS.Application.Features.Orders.Commands.UpdateOrderItemQuantity;
using POS.Application.Features.Orders.Queries.GetOrderById;
using POS.Application.Features.Orders.Queries.GetOrdersByCustomer;
using POS.Application.Features.Orders.Queries.GetAllOrders;


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

    [HttpPost]
    public async Task<IActionResult> CancelOrder([FromBody] CancelOrderCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }


    [HttpPost]
    public async Task<IActionResult> AddOrderItem([FromBody] AddOrderItemCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> RemoveOrderItem([FromBody] RemoveOrderItemCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> UpdateOrderItem([FromBody] UpdateOrderItemQuantityCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrderById(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _mediator.Send(new GetOrderByIdQuery(orderId), cancellationToken);

        return Ok(order);
    }

    [HttpGet("{customerId}")]
    public async Task<IActionResult> GetOrdersByCustomer(Guid customerId, CancellationToken cancellationToken)
    {
        var orders = await _mediator.Send(new GetOrdersByCustomerQuery(customerId), cancellationToken);
        return Ok(orders);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllOrders(CancellationToken cancellationToken)
    {
        var orders = await _mediator.Send(new GetAllOrdersQuery(), cancellationToken);
        return Ok(orders);
    }



}
