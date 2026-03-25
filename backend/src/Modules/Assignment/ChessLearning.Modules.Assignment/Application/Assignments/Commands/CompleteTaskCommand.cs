using ChessLearning.Modules.Assignment.Domain;
using ChessLearning.Modules.Assignment.Infrastructure;
using ChessLearning.Shared.Application.Contracts.Cqrs;
using Microsoft.EntityFrameworkCore;

namespace ChessLearning.Modules.Assignment.Application.Assignments.Commands;

public record CompleteTaskCommand(Guid AssignmentId, Guid TaskId) : ICommand;

public class CompleteTaskCommandHandler : ICommandHandler<CompleteTaskCommand>
{
    private readonly AssignmentDbContext _db;

    public CompleteTaskCommandHandler(AssignmentDbContext db)
    {
        _db = db;
    }

    public async Task ExecuteAsync(CompleteTaskCommand command, CancellationToken ct = default)
    {
        var assignmentTask = await _db.AssignmentTasks
            .Include(at => at.Assignment)
            .ThenInclude(a => a!.AssignmentTasks)
            .FirstOrDefaultAsync(at => at.AssignmentId == command.AssignmentId && at.TaskId == command.TaskId, ct);

        if (assignmentTask == null) throw new ArgumentException("Task not found in this assignment");
        
        if (assignmentTask.Assignment!.Status != AssignmentStatus.Active && assignmentTask.Assignment!.Status != AssignmentStatus.Overdue)
            throw new InvalidOperationException("Tasks can only be completed on Active or Overdue assignments");

        assignmentTask.MarkComplete();

        // Check if all tasks are completed
        if (assignmentTask.Assignment.AssignmentTasks.All(t => t.Status == AssignmentTaskStatus.Completed))
        {
            assignmentTask.Assignment.Complete();
        }

        await _db.SaveChangesAsync(ct);
    }
}
