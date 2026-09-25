using FluentValidation;

namespace POS.Application.Features.Orders.Commands.RemoveOrderItem;

public class RemoveOrderItemCommandValidator : AbstractValidator<RemoveOrderItemCommand>
{
    public RemoveOrderItemCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
    }
}