using Microsoft.AspNetCore.Mvc;

namespace ChessLearning.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    [HttpPost("register")]
    public IActionResult Register()
    {
        return Ok(new { Message = "Not implemented yet. Will register a new User." });
    }

    [HttpPost("login")]
    public IActionResult Login()
    {
        return Ok(new { Token = "dummy-jwt-token" });
    }

    [HttpPost("refresh")]
    public IActionResult Refresh()
    {
        return Ok(new { Token = "new-dummy-jwt-token" });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok();
    }
}
