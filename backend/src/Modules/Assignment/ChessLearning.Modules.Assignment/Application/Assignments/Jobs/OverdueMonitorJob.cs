using ChessLearning.Modules.Assignment.Domain;
using ChessLearning.Modules.Assignment.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ChessLearning.Modules.Assignment.Application.Assignments.Jobs;

public class OverdueMonitorJob
{
    private readonly AssignmentDbContext _db;

    public OverdueMonitorJob(AssignmentDbContext db)
    {
        _db = db;
    }

    public async Task ExecuteAsync()
    {
        var now = DateTime.UtcNow;

        var overdueAssignments = await _db.Assignments
            .Where(a => a.Status == AssignmentStatus.Active && now > a.DueDate)
            .ToListAsync();

        foreach (var assignment in overdueAssignments)
        {
            assignment.MarkOverdue();
        }

        if (overdueAssignments.Any())
        {
            await _db.SaveChangesAsync();
        }
    }
}
