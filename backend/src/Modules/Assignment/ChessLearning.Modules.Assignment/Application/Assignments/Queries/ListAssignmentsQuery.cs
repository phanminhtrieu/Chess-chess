using ChessLearning.Modules.Assignment.Application.Assignments;
using ChessLearning.Modules.Assignment.Domain;
using ChessLearning.Modules.Assignment.Infrastructure;
using ChessLearning.Shared.Application.Contracts.Cqrs;
using Microsoft.EntityFrameworkCore;

namespace ChessLearning.Modules.Assignment.Application.Assignments.Queries;

public record ListAssignmentsQuery(AssignmentStatus? StatusFilter) : IQuery<IEnumerable<AssignmentDto>>;

public class ListAssignmentsQueryHandler : IQueryHandler<ListAssignmentsQuery, IEnumerable<AssignmentDto>>
{
    private readonly AssignmentDbContext _db;

    public ListAssignmentsQueryHandler(AssignmentDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<AssignmentDto>> HandleAsync(ListAssignmentsQuery query, CancellationToken ct = default)
    {
        var dbQuery = _db.Assignments
            .Include(a => a.AssignmentTasks)
            .ThenInclude(at => at.Task)
            .AsNoTracking();

        if (query.StatusFilter.HasValue)
            dbQuery = dbQuery.Where(a => a.Status == query.StatusFilter.Value);

        var result = await dbQuery.Select(a => new AssignmentDto(
            a.Id,
            a.Title,
            a.DueDate,
            a.Status,
            a.AssignmentTasks.Select(at => new AssignmentTaskDto(
                at.TaskId,
                at.Task != null ? at.Task.Title : "Untitled",
                at.Status,
                at.Status == AssignmentTaskStatus.Completed
            ))
        )).ToListAsync(ct);

        return result;
    }
}
