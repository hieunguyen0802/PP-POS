using MediatR;
using POS.Application.DTOs.User;

namespace POS.Application.Features.Users.Queries.GetAllUsers;

public record GetAllUsersQuery() : IRequest<List<UserDto>>;
