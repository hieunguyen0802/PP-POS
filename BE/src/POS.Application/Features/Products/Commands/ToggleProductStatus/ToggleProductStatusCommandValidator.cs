using FluentValidation;
using MediatR;

namespace POS.Application.Features.Products.Commands.ToggleProductStatus;

public class ToggleProductStatusCommandValidator : AbstractValidator<ToggleProductStatusCommand>
{
    public ToggleProductStatusCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
    }
}