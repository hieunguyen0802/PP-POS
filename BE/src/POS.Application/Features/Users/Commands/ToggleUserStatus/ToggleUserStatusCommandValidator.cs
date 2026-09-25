using FluentValidation;

namespace POS.Application.Features.Users.Commands.ToggleUserStatus;

public class ToggleUserStatusCommandValidator : AbstractValidator<ToggleUserStatusCommand>
{
    public ToggleUserStatusCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
