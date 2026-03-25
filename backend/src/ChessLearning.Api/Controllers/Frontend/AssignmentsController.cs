using ChessLearning.Modules.Assignment.Application.Assignments;
using ChessLearning.Modules.Assignment.Application.Assignments.Commands;
using ChessLearning.Modules.Assignment.Application.Assignments.Queries;
using ChessLearning.Modules.Assignment.Domain;
using ChessLearning.Shared.Application.Contracts.Cqrs;
using Microsoft.AspNetCore.Mvc;

namespace ChessLearning.Api.Controllers.Frontend;

[ApiController]
[Route("api/frontend/assignments")]
public class AssignmentsController : ControllerBase
{
    private readonly ICommandHandler<CreateAssignmentCommand, Guid> _createHandler;
    private readonly IQueryHandler<GetAssignmentDetailQuery, AssignmentDto?> _detailHandler;
    private readonly IQueryHandler<ListAssignmentsQuery, IEnumerable<AssignmentDto>> _listHandler;
    private readonly ICommandHandler<CompleteTaskCommand> _completeTaskHandler;

    public AssignmentsController(
        ICommandHandler<CreateAssignmentCommand, Guid> createHandler,
        IQueryHandler<GetAssignmentDetailQuery, AssignmentDto?> detailHandler,
        IQueryHandler<ListAssignmentsQuery, IEnumerable<AssignmentDto>> listHandler,
        ICommandHandler<CompleteTaskCommand> completeTaskHandler)
    {
        _createHandler = createHandler;
        _detailHandler = detailHandler;
        _listHandler = listHandler;
        _completeTaskHandler = completeTaskHandler;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAssignment([FromBody] CreateAssignmentRequest request)
    {
        var id = await _createHandler.ExecuteAsync(new CreateAssignmentCommand(request.CreatedById, request.Title, request.DueDate));
        return Ok(new { id });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAssignmentDetail(Guid id)
    {
        var result = await _detailHandler.HandleAsync(new GetAssignmentDetailQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> ListAssignments([FromQuery] AssignmentStatus? status)
    {
        var result = await _listHandler.HandleAsync(new ListAssignmentsQuery(status));
        return Ok(result);
    }

    [HttpPost("{id}/tasks/{taskId}/complete")]
    public async Task<IActionResult> CompleteTask(Guid id, Guid taskId)
    {
        await _completeTaskHandler.ExecuteAsync(new CompleteTaskCommand(id, taskId));
        return Ok();
    }
}
