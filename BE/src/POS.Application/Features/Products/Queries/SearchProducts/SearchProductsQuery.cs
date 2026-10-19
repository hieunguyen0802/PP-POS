using MediatR;
using POS.Application.DTOs.Product;

namespace POS.Application.Features.Products.Queries.SearchProducts;

public record SearchProductsQuery(string Term, int Limit) : IRequest<List<ProductDto>>;