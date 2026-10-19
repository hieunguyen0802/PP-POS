
using MediatR;
using Microsoft.AspNetCore.Mvc;
using POS.Application.Features.Products.Commands.CreateProduct;
using POS.Application.Features.Products.Commands.UpdateProduct;
using POS.Application.Features.Products.Commands.ToggleProductStatus;
using POS.Application.Features.Products.Queries.GetProductById;
using POS.Application.Features.Products.Queries.GetAllProducts;
using POS.Application.Features.Products.Queries.SearchProducts;

using Microsoft.AspNetCore.Authorization;


namespace POS.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(AuthenticationSchemes = "Staff", Roles = "Manager,Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
    {
        var productId = await _mediator.Send(command, cancellationToken);
        return Ok(productId);
    }

    [Authorize(AuthenticationSchemes = "Staff", Roles = "Manager,Admin")]
    [HttpPost]
    public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }


    [HttpGet("{productId}")]
    [Authorize(AuthenticationSchemes = "Staff", Policy = "ProductReader")]
    public async Task<IActionResult> GetProductById(Guid productId, CancellationToken cancellationToken)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(productId), cancellationToken);

        return Ok(product);
    }

    [Authorize(AuthenticationSchemes = "Staff", Roles = "Manager,Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllProducts(CancellationToken cancellationToken)
    {
        var products = await _mediator.Send(new GetAllProductsQuery(), cancellationToken);
        return Ok(products);
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = "Staff", Policy = "ProductReader")]
    public async Task<IActionResult> Search([FromQuery] string name, [FromQuery] int limit = 10, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("name is required");

        var products = await _mediator.Send(new SearchProductsQuery(name, limit), cancellationToken);
        return Ok(products);
    }

    [Authorize(AuthenticationSchemes = "Staff", Roles = "Manager,Admin")]
    [HttpPost]
    public async Task<IActionResult> ToggleProductStatus([FromBody] ToggleProductStatusCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }


}
