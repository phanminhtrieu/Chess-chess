using ChessLearning.Modules.Assignment.Domain;
using ChessLearning.Modules.Assignment.Infrastructure;
using ChessLearning.Shared.Application.Contracts.Cqrs;
using Microsoft.EntityFrameworkCore;

namespace ChessLearning.Modules.Assignment.Application.Assignments.Commands;

public record HandleOverdueWorkCommand(Guid AssignmentId) : ICommand;

public class HandleOverdueWorkCommandHandler : ICommandHandler<HandleOverdueWorkCommand>
{
    private readonly AssignmentDbContext _db;

    public HandleOverdueWorkCommandHandler(AssignmentDbContext db)
    {
        _db = db;
    }

    public async Task ExecuteAsync(HandleOverdueWorkCommand command, CancellationToken ct = default)
    {
        var assignment = await _db.Assignments
            .FirstOrDefaultAsync(a => a.Id == command.AssignmentId, ct);

        if (assignment == null) throw new ArgumentException("Assignment not found");

        assignment.MarkOverdue();
        await _db.SaveChangesAsync(ct);
    }
}
