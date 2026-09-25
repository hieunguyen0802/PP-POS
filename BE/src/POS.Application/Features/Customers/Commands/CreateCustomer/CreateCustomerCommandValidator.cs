using FluentValidation;

namespace POS.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.CustomerName).NotEmpty();
        RuleFor(x => x.CustomerPhone).NotEmpty();
        RuleFor(x => x.CustomerEmail).NotEmpty().EmailAddress();
    }
}
