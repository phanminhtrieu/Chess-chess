using Microsoft.AspNetCore.Mvc;

namespace ChessLearning.Api.Controllers;

[ApiController]
[Route("api/progress")]
public class ProgressController : ControllerBase
{
    [HttpGet("summary")]
    public IActionResult GetSummary()
    {
        return Ok(new { TotalPuzzles = 100, Accuracy = 0.85, Streak = 5 });
    }

    [HttpGet("events")]
    public IActionResult GetEvents()
    {
        return Ok(new[] { new { Type = "PuzzleSolved", Value = 1 } });
    }
}
