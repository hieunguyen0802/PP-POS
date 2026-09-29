using MediatR;

namespace POS.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(string ProductName, decimal UnitPrice, string ProductBarcode) : IRequest<Guid>;