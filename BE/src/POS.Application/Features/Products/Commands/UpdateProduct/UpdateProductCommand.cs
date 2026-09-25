using MediatR;

namespace POS.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand(Guid ProductId, string ProductName, decimal UnitPrice) : IRequest<Unit>;