using ChessLearning.Modules.Assignment.Application.Assignments;
using ChessLearning.Modules.Assignment.Application.Assignments.Commands;
using ChessLearning.Modules.Assignment.Application.Assignments.Queries;
using ChessLearning.Modules.Assignment.Domain;
using ChessLearning.Shared.Application.Contracts.Cqrs;
using Microsoft.AspNetCore.Mvc;

namespace ChessLearning.Api.Controllers.Backoffice;

[ApiController]
[Route("api/backoffice/assignments")]
public class AssignmentsController : ControllerBase
{
    private readonly ICommandHandler<CreateAssignmentCommand, Guid> _createHandler;
    private readonly ICommandHandler<AddTaskToAssignmentCommand> _addTaskHandler;
    private readonly ICommandHandler<ActivateAssignmentCommand> _activateHandler;
    private readonly ICommandHandler<CompleteTaskCommand> _completeTaskHandler;
    private readonly ICommandHandler<FinalizeAssignmentCommand> _finalizeHandler;
    private readonly ICommandHandler<HandleOverdueWorkCommand> _handleOverdueHandler;
    
    private readonly IQueryHandler<AssignmentDashboardQuery, AssignmentDashboardDto> _dashboardHandler;
    private readonly IQueryHandler<GetAssignmentDetailQuery, AssignmentDto?> _detailHandler;
    private readonly IQueryHandler<TaskListInspectionQuery, IEnumerable<TaskInspectionDto>> _inspectionHandler;
    private readonly IQueryHandler<ContentCorrelationViewQuery, IEnumerable<ContentCorrelationDto>> _correlationHandler;

    public AssignmentsController(
        ICommandHandler<CreateAssignmentCommand, Guid> createHandler,
        ICommandHandler<AddTaskToAssignmentCommand> addTaskHandler,
        ICommandHandler<ActivateAssignmentCommand> activateHandler,
        ICommandHandler<CompleteTaskCommand> completeTaskHandler,
        ICommandHandler<FinalizeAssignmentCommand> finalizeHandler,
        ICommandHandler<HandleOverdueWorkCommand> handleOverdueHandler,
        IQueryHandler<AssignmentDashboardQuery, AssignmentDashboardDto> dashboardHandler,
        IQueryHandler<GetAssignmentDetailQuery, AssignmentDto?> detailHandler,
        IQueryHandler<TaskListInspectionQuery, IEnumerable<TaskInspectionDto>> inspectionHandler,
        IQueryHandler<ContentCorrelationViewQuery, IEnumerable<ContentCorrelationDto>> correlationHandler)
    {
        _createHandler = createHandler;
        _addTaskHandler = addTaskHandler;
        _activateHandler = activateHandler;
        _completeTaskHandler = completeTaskHandler;
        _finalizeHandler = finalizeHandler;
        _handleOverdueHandler = handleOverdueHandler;
        _dashboardHandler = dashboardHandler;
        _detailHandler = detailHandler;
        _inspectionHandler = inspectionHandler;
        _correlationHandler = correlationHandler;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAssignment([FromBody] CreateAssignmentRequest request)
    {
        var id = await _createHandler.ExecuteAsync(new CreateAssignmentCommand(request.CreatedById, request.Title, request.DueDate));
        return Ok(new { id });
    }

    [HttpPost("{id}/tasks")]
    public async Task<IActionResult> AddTaskToAssignment(Guid id, [FromBody] AddTaskToAssignmentDto taskDto)
    {
        await _addTaskHandler.ExecuteAsync(new AddTaskToAssignmentCommand(id, taskDto.Title, taskDto.Instructions, Guid.Empty, ContentType.Custom));
        return Ok();
    }

    [HttpPost("{id}/activate")]
    public async Task<IActionResult> ActivateAssignment(Guid id)
    {
        await _activateHandler.ExecuteAsync(new ActivateAssignmentCommand(id));
        return Ok();
    }

    [HttpPost("{id}/tasks/{taskId}/complete")]
    public async Task<IActionResult> CompleteTask(Guid id, Guid taskId)
    {
        await _completeTaskHandler.ExecuteAsync(new CompleteTaskCommand(id, taskId));
        return Ok();
    }

    [HttpPost("{id}/finalize")]
    public async Task<IActionResult> FinalizeAssignment(Guid id)
    {
        await _finalizeHandler.ExecuteAsync(new FinalizeAssignmentCommand(id));
        return Ok();
    }

    [HttpPost("{id}/mark-overdue")]
    public async Task<IActionResult> HandleOverdueWork(Guid id)
    {
        await _handleOverdueHandler.ExecuteAsync(new HandleOverdueWorkCommand(id));
        return Ok();
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> AssignmentDashboard()
    {
        var result = await _dashboardHandler.HandleAsync(new AssignmentDashboardQuery());
        return Ok(result);
    }

    [HttpGet("{id}/progress")]
    public async Task<IActionResult> DetailedProgressReview(Guid id)
    {
        var result = await _detailHandler.HandleAsync(new GetAssignmentDetailQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("{id}/tasks")]
    public async Task<IActionResult> TaskListInspection(Guid id)
    {
        var result = await _inspectionHandler.HandleAsync(new TaskListInspectionQuery(id));
        return Ok(result);
    }

    [HttpGet("{id}/content-map")]
    public async Task<IActionResult> ContentCorrelationView(Guid id)
    {
        var result = await _correlationHandler.HandleAsync(new ContentCorrelationViewQuery(id));
        return Ok(result);
    }
}
