using MediatR;
using POS.Domain.Exceptions;
using POS.Domain.Interfaces;

namespace POS.Application.Features.Users.Commands.ToggleUserStatus;

public class ToggleUserStatusCommandHandler : IRequestHandler<ToggleUserStatusCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ToggleUserStatusCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ToggleUserStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null) throw new DomainExceptions.EntityNotFound("User", request.UserId);

        user.ChangeActiveStatus();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
