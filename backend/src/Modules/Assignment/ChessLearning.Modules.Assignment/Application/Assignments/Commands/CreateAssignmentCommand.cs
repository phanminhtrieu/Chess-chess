using Domain = ChessLearning.Modules.Assignment.Domain;
using ChessLearning.Modules.Assignment.Infrastructure;
using ChessLearning.Shared.Application.Contracts.Cqrs;

namespace ChessLearning.Modules.Assignment.Application.Assignments.Commands;

public record CreateAssignmentCommand(string CreatedById, string Title, DateTime DueDate) : ICommand<Guid>;

public class CreateAssignmentCommandHandler : ICommandHandler<CreateAssignmentCommand, Guid>
{
    private readonly AssignmentDbContext _db;

    public CreateAssignmentCommandHandler(AssignmentDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> ExecuteAsync(CreateAssignmentCommand command, CancellationToken ct = default)
    {
        var assignment = Domain.Assignment.Create(command.CreatedById, command.Title, command.DueDate);
        _db.Assignments.Add(assignment);
        await _db.SaveChangesAsync(ct);
        return assignment.Id;
    }
}
