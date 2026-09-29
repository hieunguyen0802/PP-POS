

using MediatR;
using Microsoft.AspNetCore.Mvc;
using POS.Application.Features.Users.Commands.CreateUser;
using POS.Application.Features.Users.Commands.ChangeUserRole;
using POS.Application.Features.Users.Commands.ToggleUserStatus;
using POS.Application.Features.Users.Commands.UpdateUser;
using POS.Application.Features.Users.Queries.GetUserById;
using POS.Application.Features.Users.Queries.GetAllUsers;
using Microsoft.AspNetCore.Authorization;


namespace POS.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize(AuthenticationSchemes = "Staff", Roles = "Manager,Admin")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command, CancellationToken cancellationToken)
    {
        var userId = await _mediator.Send(command, cancellationToken);
        return Ok(userId);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _mediator.Send(new GetUserByIdQuery(userId), cancellationToken);

        return Ok(user);
    }


    [HttpGet]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var users = await _mediator.Send(new GetAllUsersQuery(), cancellationToken);
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleUserStatus([FromBody] ToggleUserStatusCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
    [Authorize(AuthenticationSchemes = "Staff", Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> ChangeUserRole([FromBody] ChangeUserRoleCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }


}
