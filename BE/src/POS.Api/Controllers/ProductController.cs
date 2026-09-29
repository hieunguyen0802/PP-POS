
using MediatR;
using Microsoft.AspNetCore.Mvc;
using POS.Application.Features.Products.Commands.CreateProduct;
using POS.Application.Features.Products.Commands.UpdateProduct;
using POS.Application.Features.Products.Commands.ToggleProductStatus;
using POS.Application.Features.Products.Queries.GetProductById;
using POS.Application.Features.Products.Queries.GetAllProducts;
using Microsoft.AspNetCore.Authorization;


namespace POS.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize(AuthenticationSchemes = "Staff", Roles = "Manager,Admin")]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
    {
        var productId = await _mediator.Send(command, cancellationToken);
        return Ok(productId);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProductById(Guid productId, CancellationToken cancellationToken)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(productId), cancellationToken);

        return Ok(product);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts(CancellationToken cancellationToken)
    {
        var products = await _mediator.Send(new GetAllProductsQuery(), cancellationToken);
        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleProductStatus([FromBody] ToggleProductStatusCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }


}
