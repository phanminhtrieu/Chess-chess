using Microsoft.AspNetCore.Mvc;

namespace ChessLearning.Api.Controllers;

[ApiController]
[Route("api/games")]
public class GamesController : ControllerBase
{
    [HttpGet]
    public IActionResult GetGames()
    {
        return Ok(new[] { new { ExternalId = "game_1", Opponent = "MagnusCarlsen" } });
    }

    [HttpPost("import")]
    public IActionResult ImportGames([FromBody] string username)
    {
        // Queues via Hangfire in backend
        return Accepted(new { Message = $"Import started for {username}" });
    }
}
