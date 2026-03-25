using ChessLearning.Modules.Assignment.Application.Assignments;
using ChessLearning.Modules.Assignment.Domain;
using ChessLearning.Modules.Assignment.Infrastructure;
using ChessLearning.Shared.Application.Contracts.Cqrs;
using Microsoft.EntityFrameworkCore;

namespace ChessLearning.Modules.Assignment.Application.Assignments.Queries;

public record GetAssignmentDetailQuery(Guid AssignmentId) : IQuery<AssignmentDto?>;

public class GetAssignmentDetailQueryHandler : IQueryHandler<GetAssignmentDetailQuery, AssignmentDto?>
{
    private readonly AssignmentDbContext _db;

    public GetAssignmentDetailQueryHandler(AssignmentDbContext db)
    {
        _db = db;
    }

    public async Task<AssignmentDto?> HandleAsync(GetAssignmentDetailQuery query, CancellationToken ct = default)
    {
        var assignment = await _db.Assignments
            .Include(a => a.AssignmentTasks)
            .ThenInclude(at => at.Task)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == query.AssignmentId, ct);

        if (assignment == null) return null;

        return new AssignmentDto(
            assignment.Id,
            assignment.Title,
            assignment.DueDate,
            assignment.Status,
            assignment.AssignmentTasks.OrderBy(at => at.Order).Select(at => new AssignmentTaskDto(
                at.TaskId,
                at.Task?.Title ?? "Untitled",
                at.Status,
                at.Status == AssignmentTaskStatus.Completed
            ))
        );
    }
}
