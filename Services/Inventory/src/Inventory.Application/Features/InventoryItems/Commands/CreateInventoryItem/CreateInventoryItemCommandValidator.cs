using FluentValidation;

namespace Inventory.Application.Features.InventoryItems.Commands.CreateInventoryItem;

public class CreateInventoryItemCommandValidator : AbstractValidator<CreateInventoryItemCommand>
{
    public CreateInventoryItemCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product id is required.");

        RuleFor(x => x.InitialQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Initial quantity cannot be negative.");

        RuleFor(x => x.ReorderThreshold)
            .GreaterThanOrEqualTo(0).WithMessage("Reorder threshold cannot be negative.");
    }
}
