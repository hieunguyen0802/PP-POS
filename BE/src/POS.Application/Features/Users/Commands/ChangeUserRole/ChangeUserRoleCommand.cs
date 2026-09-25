using MediatR;
using POS.Domain.Entities;

namespace POS.Application.Features.Users.Commands.ChangeUserRole;

public record ChangeUserRoleCommand(Guid UserId, UserRole NewRole) : IRequest<Unit>;
