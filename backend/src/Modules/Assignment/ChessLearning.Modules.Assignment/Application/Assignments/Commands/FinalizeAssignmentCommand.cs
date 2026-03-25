using ChessLearning.Modules.Assignment.Domain;
using ChessLearning.Modules.Assignment.Infrastructure;
using ChessLearning.Shared.Application.Contracts.Cqrs;
using Microsoft.EntityFrameworkCore;

namespace ChessLearning.Modules.Assignment.Application.Assignments.Commands;

public record FinalizeAssignmentCommand(Guid AssignmentId) : ICommand;

public class FinalizeAssignmentCommandHandler : ICommandHandler<FinalizeAssignmentCommand>
{
    private readonly AssignmentDbContext _db;

    public FinalizeAssignmentCommandHandler(AssignmentDbContext db)
    {
        _db = db;
    }

    public async Task ExecuteAsync(FinalizeAssignmentCommand command, CancellationToken ct = default)
    {
        var assignment = await _db.Assignments
            .Include(a => a.AssignmentTasks)
            .FirstOrDefaultAsync(a => a.Id == command.AssignmentId, ct);

        if (assignment == null) throw new ArgumentException("Assignment not found");

        if (assignment.AssignmentTasks.Any(at => at.Status != AssignmentTaskStatus.Completed))
            throw new InvalidOperationException("Cannot finalize assignment with incomplete tasks");

        assignment.Complete();
        await _db.SaveChangesAsync(ct);
    }
}
