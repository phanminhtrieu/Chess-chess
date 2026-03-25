using ChessLearning.Modules.Assignment.Domain;
using ChessLearning.Modules.Assignment.Infrastructure;
using ChessLearning.Shared.Application.Contracts.Cqrs;
using Microsoft.EntityFrameworkCore;

namespace ChessLearning.Modules.Assignment.Application.Assignments.Commands;

public record AddTaskToAssignmentCommand(Guid AssignmentId, string Title, string Instructions, Guid ContentId, ContentType ContentType) : ICommand;

public class AddTaskToAssignmentCommandHandler : ICommandHandler<AddTaskToAssignmentCommand>
{
    private readonly AssignmentDbContext _db;

    public AddTaskToAssignmentCommandHandler(AssignmentDbContext db)
    {
        _db = db;
    }

    public async Task ExecuteAsync(AddTaskToAssignmentCommand command, CancellationToken ct = default)
    {
        var assignment = await _db.Assignments
            .Include(a => a.AssignmentTasks)
            .FirstOrDefaultAsync(a => a.Id == command.AssignmentId, ct);

        if (assignment == null) throw new ArgumentException("Assignment not found");

        var domainTask = new LearningTask
        {
            Title = command.Title,
            Instructions = command.Instructions,
            ContentId = command.ContentId,
            ContentType = command.ContentType,
            CreatedById = assignment.CreatedById,
            IsPublic = true
        };

        _db.Tasks.Add(domainTask);
        
        assignment.AddTask(domainTask, assignment.AssignmentTasks.Count + 1);

        await _db.SaveChangesAsync(ct);
    }
}
