using Microsoft.AspNetCore.Mvc;

namespace ChessLearning.Api.Controllers;

[ApiController]
[Route("api/assignments")]
public class AssignmentsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAssignments()
    {
        return Ok(new[] { new { Title = "Assignment 1" } });
    }

    [HttpPost]
    public IActionResult CreateAssignment()
    {
        return Ok(new { Message = "Assignment created." });
    }

    [HttpGet("{id}/tasks")]
    public IActionResult GetTasks(Guid id)
    {
        return Ok(new[] { new { TaskId = Guid.NewGuid(), Title = "Solve 3 puzzles" } });
    }
}
