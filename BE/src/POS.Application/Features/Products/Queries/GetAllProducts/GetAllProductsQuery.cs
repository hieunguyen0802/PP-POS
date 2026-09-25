using MediatR;
using POS.Application.DTOs.Product;

namespace POS.Application.Features.Products.Queries.GetAllProducts;

public record GetAllProductsQuery() : IRequest<List<ProductDto>>;
