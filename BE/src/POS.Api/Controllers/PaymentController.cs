

using MediatR;
using Microsoft.AspNetCore.Mvc;
using POS.Application.Features.Payments.Commands.CreatePayment;
using POS.Application.Features.Payments.Queries.GetPaymentById;
using POS.Application.Features.Payments.Queries.GetAllPayments;
using POS.Application.Features.Payments.Queries.GetPaymentsByOrderId;


namespace POS.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]

public class PaymentController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentCommand command, CancellationToken cancellationToken)
    {
        var paymentId = await _mediator.Send(command, cancellationToken);
        return Ok(paymentId);
    }



    [HttpGet("{paymentId}")]
    public async Task<IActionResult> GetPaymentById(Guid paymentId, CancellationToken cancellationToken)
    {
        var payment = await _mediator.Send(new GetPaymentByIdQuery(paymentId), cancellationToken);

        return Ok(payment);
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetPaymentsByOrderId(Guid orderId, CancellationToken cancellationToken)
    {
        var payments = await _mediator.Send(new GetPaymentsByOrderIdQuery(orderId), cancellationToken);

        return Ok(payments);
    }


    [HttpGet]
    public async Task<IActionResult> GetAllPayments(CancellationToken cancellationToken)
    {
        var payments = await _mediator.Send(new GetAllPaymentsQuery(), cancellationToken);
        return Ok(payments);
    }


}
