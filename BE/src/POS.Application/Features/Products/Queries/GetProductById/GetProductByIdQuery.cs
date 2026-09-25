using MediatR;
using POS.Application.DTOs.Product;

namespace POS.Application.Features.Products.Queries.GetProductById;

public record GetProductByIdQuery(Guid ProductId) : IRequest<ProductDto>;
