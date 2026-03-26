using ChessLearning.Modules.Identity.Application.Users;
using ChessLearning.Modules.Identity.Application.Users.Commands;
using ChessLearning.Modules.Identity.Application.Users.Queries;
using ChessLearning.Shared.Application.Contracts.Cqrs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChessLearning.Api.Controllers.Backoffice;

[ApiController]
[Route("api/backoffice/users")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IQueryHandler<ListUsersQuery, IEnumerable<UserListItemDto>> _listHandler;
    private readonly ICommandHandler<UpdateUserCommand> _updateHandler;
    private readonly ICommandHandler<RevokeUserSessionsCommand> _revokeHandler;
    private readonly ICommandHandler<DeactivateUserCommand> _deactivateHandler;
    private readonly IQueryHandler<GetUserAuditQuery, UserAuditDto> _auditHandler;

    public UsersController(
        IQueryHandler<ListUsersQuery, IEnumerable<UserListItemDto>> listHandler,
        ICommandHandler<UpdateUserCommand> updateHandler,
        ICommandHandler<RevokeUserSessionsCommand> revokeHandler,
        ICommandHandler<DeactivateUserCommand> deactivateHandler,
        IQueryHandler<GetUserAuditQuery, UserAuditDto> auditHandler)
    {
        _listHandler = listHandler;
        _updateHandler = updateHandler;
        _revokeHandler = revokeHandler;
        _deactivateHandler = deactivateHandler;
        _auditHandler = auditHandler;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var result = await _listHandler.HandleAsync(new ListUsersQuery());
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest body)
    {
        await _updateHandler.ExecuteAsync(new UpdateUserCommand(id, body.DisplayName, body.Role));
        return Ok();
    }

    [HttpPost("{id}/revoke-sessions")]
    public async Task<IActionResult> RevokeSessions(Guid id)
    {
        await _revokeHandler.ExecuteAsync(new RevokeUserSessionsCommand(id));
        return Ok();
    }

    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await _deactivateHandler.ExecuteAsync(new DeactivateUserCommand(id));
        return Ok();
    }

    [HttpGet("{id}/audit")]
    public async Task<IActionResult> GetAudit(Guid id)
    {
        var result = await _auditHandler.HandleAsync(new GetUserAuditQuery(id));
        return Ok(result);
    }
}

public class UpdateUserRequest
{
    public string? DisplayName { get; set; }
    public string? Role { get; set; }
}
