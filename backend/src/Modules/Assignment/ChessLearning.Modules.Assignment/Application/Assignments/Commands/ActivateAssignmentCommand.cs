using ChessLearning.Modules.Assignment.Infrastructure;
using ChessLearning.Shared.Application.Contracts.Cqrs;
using Microsoft.EntityFrameworkCore;

namespace ChessLearning.Modules.Assignment.Application.Assignments.Commands;

public record ActivateAssignmentCommand(Guid AssignmentId) : ICommand;

public class ActivateAssignmentCommandHandler : ICommandHandler<ActivateAssignmentCommand>
{
    private readonly AssignmentDbContext _db;

    public ActivateAssignmentCommandHandler(AssignmentDbContext db)
    {
        _db = db;
    }

    public async Task ExecuteAsync(ActivateAssignmentCommand command, CancellationToken ct = default)
    {
        var assignment = await _db.Assignments
            .Include(a => a.AssignmentTasks)
            .FirstOrDefaultAsync(a => a.Id == command.AssignmentId, ct);

        if (assignment == null) throw new ArgumentException("Assignment not found");

        assignment.Activate();
        await _db.SaveChangesAsync(ct);
    }
}
