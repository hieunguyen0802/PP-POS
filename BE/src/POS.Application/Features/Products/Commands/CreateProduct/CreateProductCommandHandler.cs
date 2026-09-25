using MediatR;
using POS.Domain.Exceptions;
using POS.Domain.Interfaces;
using POS.Domain.Entities;

namespace POS.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {

        var existingProduct = await _productRepository.GetByProductCodeAsync(request.ProductCode, cancellationToken);
        if (existingProduct != null)
        {
            throw new DomainExceptions.EntityAlreadyExists("Product", request.ProductCode);
        }

        var product = new Product(productCode: request.ProductCode, productName: request.ProductName, unitPrice: request.UnitPrice, barcode: request.ProductBarcode);

        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}