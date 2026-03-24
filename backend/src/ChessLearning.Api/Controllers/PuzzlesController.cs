using Microsoft.AspNetCore.Mvc;

namespace ChessLearning.Api.Controllers;

[ApiController]
[Route("api/puzzles")]
public class PuzzlesController : ControllerBase
{
    [HttpGet]
    public IActionResult GetPuzzles([FromQuery] string? theme)
    {
        return Ok(new[] { new { FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", Theme = theme ?? "mateIn2" } });
    }

    [HttpPost("{id}/attempts")]
    public IActionResult SubmitAttempt(Guid id, [FromBody] bool success)
    {
        return Ok(new { PuzzleId = id, Success = success });
    }
}
