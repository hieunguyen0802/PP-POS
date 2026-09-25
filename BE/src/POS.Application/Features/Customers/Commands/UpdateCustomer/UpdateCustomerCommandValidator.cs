using FluentValidation;

namespace POS.Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.CustomerName).NotEmpty();
        RuleFor(x => x.CustomerPhone).NotEmpty();
        RuleFor(x => x.CustomerEmail).NotEmpty().EmailAddress();
    }
}
