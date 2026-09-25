using MediatR;
using POS.Application.DTOs.User;

namespace POS.Application.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid UserId) : IRequest<UserDto>;
