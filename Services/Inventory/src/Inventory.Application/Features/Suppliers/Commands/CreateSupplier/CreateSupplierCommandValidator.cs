// CreateSupplierCommandValidator.cs
using FluentValidation;
namespace Inventory.Application.Features.Suppliers.Commands.CreateSupplier;

public class CreateSupplierCommandValidator : AbstractValidator<CreateSupplierCommand>
{
    public CreateSupplierCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ContactInfo).NotEmpty().MaximumLength(500);
    }
}