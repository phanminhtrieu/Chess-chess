using ChessLearning.Modules.Assignment.Domain;
using ChessLearning.Modules.Assignment.Infrastructure;
using ChessLearning.Shared.Application.Contracts.Cqrs;
using Microsoft.EntityFrameworkCore;

namespace ChessLearning.Modules.Assignment.Application.Assignments.Queries;

public record TaskListInspectionQuery(Guid AssignmentId) : IQuery<IEnumerable<TaskInspectionDto>>;

public class TaskListInspectionQueryHandler : IQueryHandler<TaskListInspectionQuery, IEnumerable<TaskInspectionDto>>
{
    private readonly AssignmentDbContext _db;

    public TaskListInspectionQueryHandler(AssignmentDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<TaskInspectionDto>> HandleAsync(TaskListInspectionQuery query, CancellationToken ct = default)
    {
        return await _db.AssignmentTasks
            .Where(at => at.AssignmentId == query.AssignmentId)
            .OrderBy(at => at.Order)
            .Select(at => new TaskInspectionDto(
                at.Id,
                at.TaskId,
                at.Status,
                at.CreatedAt,
                at.CompletedAt,
                at.Order
            ))
            .ToListAsync(ct);
    }
}
