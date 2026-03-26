using ChessLearning.Modules.Identity.Application.Users;
using ChessLearning.Modules.Identity.Application.Users.Commands;
using ChessLearning.Modules.Identity.Application.Users.Queries;
using ChessLearning.Shared.Application.Contracts.Cqrs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChessLearning.Api.Controllers.Frontend;

[ApiController]
[Route("api/frontend/users")]
public class UsersController : ControllerBase
{
    private readonly ICommandHandler<RegisterUserCommand, Guid> _registerHandler;
    private readonly ICommandHandler<LoginCommand, LoginResultDto> _loginHandler;
    private readonly ICommandHandler<RefreshSessionCommand, LoginResultDto> _refreshHandler;
    private readonly ICommandHandler<LogoutCommand> _logoutHandler;
    private readonly IQueryHandler<GetProfileQuery, UserProfileDto> _profileHandler;

    public UsersController(
        ICommandHandler<RegisterUserCommand, Guid> registerHandler,
        ICommandHandler<LoginCommand, LoginResultDto> loginHandler,
        ICommandHandler<RefreshSessionCommand, LoginResultDto> refreshHandler,
        ICommandHandler<LogoutCommand> logoutHandler,
        IQueryHandler<GetProfileQuery, UserProfileDto> profileHandler)
    {
        _registerHandler = registerHandler;
        _loginHandler = loginHandler;
        _refreshHandler = refreshHandler;
        _logoutHandler = logoutHandler;
        _profileHandler = profileHandler;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand body)
    {
        var id = await _registerHandler.ExecuteAsync(body);
        return Ok(new { id });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand body)
    {
        var result = await _loginHandler.ExecuteAsync(body);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshSessionCommand body)
    {
        var result = await _refreshHandler.ExecuteAsync(body);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand body)
    {
        await _logoutHandler.ExecuteAsync(body);
        return Ok();
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> Profile()
    {
        var profile = await _profileHandler.HandleAsync(new GetProfileQuery());
        return Ok(profile);
    }
}
