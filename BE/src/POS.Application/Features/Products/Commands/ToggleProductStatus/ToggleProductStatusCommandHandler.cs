using MediatR;
using POS.Domain.Exceptions;
using POS.Domain.Interfaces;

namespace POS.Application.Features.Products.Commands.ToggleProductStatus;

public class ToggleProductStatusCommandHandler : IRequestHandler<ToggleProductStatusCommand, Unit>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ToggleProductStatusCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ToggleProductStatusCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null) throw new DomainExceptions.EntityNotFound("Product", request.ProductId);

        product.ChangeActiveStatus();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}