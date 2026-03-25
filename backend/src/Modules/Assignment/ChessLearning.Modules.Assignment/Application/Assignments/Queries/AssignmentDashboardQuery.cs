using ChessLearning.Modules.Assignment.Domain;
using ChessLearning.Modules.Assignment.Infrastructure;
using ChessLearning.Shared.Application.Contracts.Cqrs;
using Microsoft.EntityFrameworkCore;

namespace ChessLearning.Modules.Assignment.Application.Assignments.Queries;

public record AssignmentDashboardQuery() : IQuery<AssignmentDashboardDto>;

public class AssignmentDashboardQueryHandler : IQueryHandler<AssignmentDashboardQuery, AssignmentDashboardDto>
{
    private readonly AssignmentDbContext _db;

    public AssignmentDashboardQueryHandler(AssignmentDbContext db)
    {
        _db = db;
    }

    public async Task<AssignmentDashboardDto> HandleAsync(AssignmentDashboardQuery query, CancellationToken ct = default)
    {
        // Counts
        var statusCounts = await _db.Assignments
            .GroupBy(a => a.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count, ct);

        // Ensure all statuses represented
        foreach (var status in Enum.GetValues<AssignmentStatus>())
            if (!statusCounts.ContainsKey(status)) statusCounts[status] = 0;

        // Recent
        var recent = await _db.Assignments
            .OrderByDescending(a => a.UpdatedAt)
            .Take(10)
            .Select(a => new AssignmentDto(
                a.Id,
                a.Title,
                a.DueDate,
                a.Status,
                a.AssignmentTasks.Select(at => new AssignmentTaskDto(at.TaskId, at.Task!.Title, at.Status, at.Status == AssignmentTaskStatus.Completed))
            ))
            .ToListAsync(ct);

        return new AssignmentDashboardDto(statusCounts, recent);
    }
}
