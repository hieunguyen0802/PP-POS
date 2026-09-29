
using MediatR;
using Microsoft.AspNetCore.Mvc;
using POS.Application.Features.Customers.Commands.CreateCustomer;
using POS.Application.Features.Customers.Commands.UpdateCustomer;
using POS.Application.Features.Customers.Queries.GetCustomerById;
using POS.Application.Features.Customers.Queries.GetAllCustomers;


namespace POS.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]

public class CustomerController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerCommand command, CancellationToken cancellationToken)
    {
        var customerId = await _mediator.Send(command, cancellationToken);
        return Ok(customerId);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateCustomer([FromBody] UpdateCustomerCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }


    [HttpGet("{customerId}")]
    public async Task<IActionResult> GetCustomerById(Guid customerId, CancellationToken cancellationToken)
    {
        var customer = await _mediator.Send(new GetCustomerByIdQuery(customerId), cancellationToken);

        return Ok(customer);
    }


    [HttpGet]
    public async Task<IActionResult> GetAllCustomers(CancellationToken cancellationToken)
    {
        var customers = await _mediator.Send(new GetAllCustomersQuery(), cancellationToken);
        return Ok(customers);
    }



}
