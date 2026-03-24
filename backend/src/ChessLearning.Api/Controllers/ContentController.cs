using Microsoft.AspNetCore.Mvc;

namespace ChessLearning.Api.Controllers;

[ApiController]
[Route("api/content")]
public class ContentController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllContent([FromQuery] string? type)
    {
        return Ok(new[] { new { Title = "Basic Endgame", Type = type ?? "Custom" } });
    }

    [HttpPost]
    public IActionResult CreateContent()
    {
        return Ok(new { Message = "Content created." });
    }
}
