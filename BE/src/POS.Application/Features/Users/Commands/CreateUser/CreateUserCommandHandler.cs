using MediatR;
using POS.Application.Common.Interfaces;
using POS.Domain.Entities;
using POS.Domain.Exceptions;
using POS.Domain.Interfaces;

namespace POS.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var usernameTaken = await _userRepository.ExistsByUsernameAsync(request.Username, cancellationToken);
        if (usernameTaken)
        {
            throw new DomainExceptions.EntityAlreadyExists("User", request.Username);
        }

        var passwordHash = _passwordHasher.Hash(request.Password);

        var user = new User(request.Username, passwordHash, request.FirstName, request.LastName, request.Email, request.PhoneNumber, request.Role);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
