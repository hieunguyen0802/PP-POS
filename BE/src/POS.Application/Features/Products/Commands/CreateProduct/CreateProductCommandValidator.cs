using FluentValidation;

namespace POS.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.ProductCode)
            .NotEmpty().WithMessage("Product code is required.");

        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("Product name is required.");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0).WithMessage("Unit price must be greater than zero.");
        RuleFor(x => x.ProductBarcode)
            .NotEmpty().WithMessage("Barcode is required.");
    }
}


