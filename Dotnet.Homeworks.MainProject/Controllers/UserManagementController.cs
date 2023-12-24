using Dotnet.Homeworks.Domain.Entities;
using Dotnet.Homeworks.Features.UserManagement.Commands.DeleteUserByAdmin;
using Dotnet.Homeworks.Features.UserManagement.Queries.GetAllUsers;
using Dotnet.Homeworks.Features.Users.Commands.CreateUser;
using Dotnet.Homeworks.Features.Users.Commands.DeleteUser;
using Dotnet.Homeworks.Features.Users.Commands.UpdateUser;
using Dotnet.Homeworks.Features.Users.Queries.GetUser;
using Dotnet.Homeworks.MainProject.Dto;
using Dotnet.Homeworks.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet.Homeworks.MainProject.Controllers;

[ApiController]
public class UserManagementController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserManagementController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("user")]
    public async Task<IActionResult> CreateUserAsync(RegisterUserDto userDto, CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(userDto.Name, userDto.Email);
        var res = await _mediator.Send(command, cancellationToken);
        
        return res.IsSuccess
            ? Ok(res.Value)
            : BadRequest(res.Error);
    }

    [HttpGet("profile/{guid}")]
    public async Task<IActionResult> GetProfile(Guid guid, CancellationToken cancellationToken)
    {
        var query = new GetUserQuery(guid);
        var res = await _mediator.Send(query, cancellationToken);
        
        return res.IsSuccess
            ? Ok(res.Value)
            : BadRequest(res.Error);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var query = new GetAllUsersQuery();
        var res = await _mediator.Send(query, cancellationToken);
        
        return res.IsSuccess
            ? Ok(res.Value)
            : BadRequest(res.Error);
    }

    [HttpDelete("profile/{guid:guid}")]
    public async Task<IActionResult> DeleteProfile(Guid guid, CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand(guid);
        var res = await _mediator.Send(command, cancellationToken);
        
        return res.IsSuccess
            ? Ok()
            : BadRequest(res.Error);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(User user, CancellationToken cancellationToken)
    {
        var command = new UpdateUserCommand(user);
        var res = await _mediator.Send(command, cancellationToken);
        
        return res.IsSuccess
            ? Ok()
            : BadRequest(res.Error);
    }

    [HttpDelete("user/{guid:guid}")]
    public async Task<IActionResult> DeleteUser(Guid guid, CancellationToken cancellationToken)
    {
        var command = new DeleteUserByAdminCommand(guid);
        var res = await _mediator.Send(command, cancellationToken);
        
        return res.IsSuccess
            ? Ok()
            : BadRequest(res.Error);
    }
}