using MediatR;

namespace POS.Application.Features.Users.Commands.ToggleUserStatus;

public record ToggleUserStatusCommand(Guid UserId) : IRequest<Unit>;
