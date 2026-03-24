using Microsoft.AspNetCore.Mvc;
using ChessLearning.Modules.Learning.Domain;

namespace ChessLearning.Api.Controllers;

[ApiController]
[Route("api/learning-plans")]
public class LearningPlansController : ControllerBase
{
    [HttpGet]
    public IActionResult GetMyPlans()
    {
        return Ok(new List<LearningPlan>());
    }

    [HttpPost]
    public IActionResult CreatePlan([FromBody] LearningPlan plan)
    {
        return Created($"/api/learning-plans/{plan.Id}", plan);
    }

    [HttpGet("{id}")]
    public IActionResult GetPlan(Guid id)
    {
        return Ok(new { Id = id, Title = "Sample Plan" });
    }
}
