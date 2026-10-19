using AutoMapper;
using MediatR;
using POS.Application.DTOs.Product;
using POS.Domain.Interfaces;

namespace POS.Application.Features.Products.Queries.SearchProducts;

public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, List<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public SearchProductsQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<List<ProductDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        var limit = Math.Clamp(request.Limit, 1, 50);
        var products = await _productRepository.SearchByNameAsync(request.Term.Trim(), limit, cancellationToken);
        return _mapper.Map<List<ProductDto>>(products);
    }
}