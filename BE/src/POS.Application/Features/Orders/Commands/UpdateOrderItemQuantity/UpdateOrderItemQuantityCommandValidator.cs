
using FluentValidation;

namespace POS.Application.Features.Orders.Commands.UpdateOrderItemQuantity;

public class UpdateOrderItemQuantityCommandValidator : AbstractValidator<UpdateOrderItemQuantityCommand>
{
    public UpdateOrderItemQuantityCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}