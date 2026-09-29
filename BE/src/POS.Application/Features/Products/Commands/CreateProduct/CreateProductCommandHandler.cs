using MediatR;
using POS.Domain.Exceptions;
using POS.Domain.Interfaces;
using POS.Domain.Entities;

namespace POS.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly ICodeGenerator _codeGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(
        IProductRepository productRepository, ICodeGenerator codeGenerator,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _codeGenerator = codeGenerator;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var productCode = await _codeGenerator.GenerateCodeAsync("PROD", cancellationToken);
        var product = new Product(productCode: productCode, productName: request.ProductName, unitPrice: request.UnitPrice, barcode: request.ProductBarcode);

        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}