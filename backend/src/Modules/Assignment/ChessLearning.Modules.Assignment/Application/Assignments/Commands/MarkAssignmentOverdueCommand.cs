using ChessLearning.Modules.Assignment.Infrastructure;
using ChessLearning.Shared.Application.Contracts.Cqrs;

namespace ChessLearning.Modules.Assignment.Application.Assignments.Commands;

public record MarkAssignmentOverdueCommand(Guid AssignmentId) : ICommand;

public class MarkAssignmentOverdueCommandHandler : ICommandHandler<MarkAssignmentOverdueCommand>
{
    private readonly AssignmentDbContext _db;

    public MarkAssignmentOverdueCommandHandler(AssignmentDbContext db)
    {
        _db = db;
    }

    public async Task ExecuteAsync(MarkAssignmentOverdueCommand command, CancellationToken ct = default)
    {
        var assignment = await _db.Assignments.FindAsync(new object[] { command.AssignmentId }, ct);
        if (assignment == null) return;

        assignment.MarkOverdue();
        await _db.SaveChangesAsync(ct);
    }
}
