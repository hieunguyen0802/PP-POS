using MediatR;
using POS.Domain.Exceptions;
using POS.Domain.Interfaces;
using POS.Domain.Entities;

namespace POS.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null) throw new DomainExceptions.EntityNotFound("Product", request.ProductId);

        product.UpdateProduct(request.ProductName, request.UnitPrice);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}