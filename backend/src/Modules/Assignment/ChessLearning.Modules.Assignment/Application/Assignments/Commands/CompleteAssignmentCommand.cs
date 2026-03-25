using ChessLearning.Modules.Assignment.Infrastructure;
using ChessLearning.Shared.Application.Contracts.Cqrs;
using Microsoft.EntityFrameworkCore;

namespace ChessLearning.Modules.Assignment.Application.Assignments.Commands;

public record CompleteAssignmentCommand(Guid AssignmentId) : ICommand;

public class CompleteAssignmentCommandHandler : ICommandHandler<CompleteAssignmentCommand>
{
    private readonly AssignmentDbContext _db;

    public CompleteAssignmentCommandHandler(AssignmentDbContext db)
    {
        _db = db;
    }

    public async Task ExecuteAsync(CompleteAssignmentCommand command, CancellationToken ct = default)
    {
        var assignment = await _db.Assignments
            .Include(a => a.AssignmentTasks)
            .FirstOrDefaultAsync(a => a.Id == command.AssignmentId, ct);

        if (assignment == null) throw new ArgumentException("Assignment not found");

        assignment.Complete();
        await _db.SaveChangesAsync(ct);
    }
}
