using AutoMapper;
using MediatR;
using POS.Application.DTOs.Product;
using POS.Domain.Exceptions;
using POS.Domain.Interfaces;

namespace POS.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null) throw new DomainExceptions.EntityNotFound("Product", request.ProductId);
        return _mapper.Map<ProductDto>(product);
    }
}