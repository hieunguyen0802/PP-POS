using MediatR;
using POS.Domain.Entities;

namespace POS.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand(string Username, string Password, string FirstName, string LastName, string Email, string PhoneNumber, UserRole Role) : IRequest<Guid>;
