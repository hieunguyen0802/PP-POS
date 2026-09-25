using MediatR;

namespace POS.Application.Features.Users.Commands.UpdateUser;

public record UpdateUserCommand(Guid UserId, string FirstName, string LastName, string Email, string PhoneNumber) : IRequest<Unit>;
