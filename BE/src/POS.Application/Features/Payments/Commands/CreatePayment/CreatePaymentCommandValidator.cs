using FluentValidation;

namespace POS.Application.Features.Payments.Commands.CreatePayment;

public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.PaymentMethod).IsInEnum();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}
