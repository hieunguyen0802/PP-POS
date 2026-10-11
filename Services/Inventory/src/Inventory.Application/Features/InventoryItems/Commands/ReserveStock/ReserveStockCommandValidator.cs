using FluentValidation;

namespace Inventory.Application.Features.InventoryItems.Commands.ReserveStock;

public class ReserveStockCommandValidator : AbstractValidator<ReserveStockCommand>
{
    public ReserveStockCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product id is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity to reserve must be greater than zero.");
    }
}