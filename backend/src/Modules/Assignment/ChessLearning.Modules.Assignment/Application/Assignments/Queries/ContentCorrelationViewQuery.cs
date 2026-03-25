using ChessLearning.Modules.Assignment.Domain;
using ChessLearning.Modules.Assignment.Infrastructure;
using ChessLearning.Shared.Application.Contracts.Cqrs;
using Microsoft.EntityFrameworkCore;

namespace ChessLearning.Modules.Assignment.Application.Assignments.Queries;

public record ContentCorrelationViewQuery(Guid AssignmentId) : IQuery<IEnumerable<ContentCorrelationDto>>;

public class ContentCorrelationViewQueryHandler : IQueryHandler<ContentCorrelationViewQuery, IEnumerable<ContentCorrelationDto>>
{
    private readonly AssignmentDbContext _db;

    public ContentCorrelationViewQueryHandler(AssignmentDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<ContentCorrelationDto>> HandleAsync(ContentCorrelationViewQuery query, CancellationToken ct = default)
    {
        return await _db.AssignmentTasks
            .Where(at => at.AssignmentId == query.AssignmentId)
            .Include(at => at.Task)
            .OrderBy(at => at.Order)
            .Select(at => new ContentCorrelationDto(
                at.TaskId,
                at.Task != null ? at.Task.ContentId : Guid.Empty,
                at.Task != null ? at.Task.ContentType.ToString() : "Unknown",
                at.Task != null ? at.Task.Title : "Untitled"
            ))
            .ToListAsync(ct);
    }
}
