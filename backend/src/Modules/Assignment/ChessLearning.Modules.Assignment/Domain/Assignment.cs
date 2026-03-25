using ChessLearning.Shared.Domain;

namespace ChessLearning.Modules.Assignment.Domain;

public enum AssignmentStatus
{
    Draft,
    Active,
    Completed,
    Overdue
}

public class Assignment : AggregateRoot
{
    public string CreatedById { get; set; } = default!;
    public DateTime? CompletedAt { get; private set; }

    public string Title { get; private set; } = default!;
    public DateTime DueDate { get; private set; }
    public AssignmentStatus Status { get; private set; } = AssignmentStatus.Draft;

    public ICollection<AssignmentTask> AssignmentTasks { get; private set; } = new List<AssignmentTask>();

    private Assignment() { }

    public static Assignment Create(string createdById, string title, DateTime dueDate)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title must not be empty");
        // if (dueDate <= DateTime.UtcNow) throw new ArgumentException("DueDate must be in the future"); // spec says just "must be valid datetime"

        return new Assignment
        {
            Title = title,
            DueDate = dueDate,
            Status = AssignmentStatus.Draft,
            CreatedById = createdById
        };
    }

    public void AddTask(LearningTask task, int order)
    {
        if (Status != AssignmentStatus.Draft)
            throw new InvalidOperationException("Tasks can only be added to a Draft assignment");

        if (string.IsNullOrWhiteSpace(task.Title))
            throw new ArgumentException("Task Title must not be empty");

        AssignmentTasks.Add(new AssignmentTask
        {
            AssignmentId = this.Id,
            TaskId = task.Id,
            Order = order,
            Task = task
        });
    }

    public void Activate()
    {
        if (Status != AssignmentStatus.Draft)
            throw new InvalidOperationException("Only Draft assignments can be activated");

        if (!AssignmentTasks.Any())
            throw new InvalidOperationException("Assignments must have at least one task to be activated");

        Status = AssignmentStatus.Active;
        SetUpdatedAt();
    }

    public void MarkOverdue()
    {
        if (Status == AssignmentStatus.Active && DateTime.UtcNow > DueDate)
        {
            Status = AssignmentStatus.Overdue;
            SetUpdatedAt();
        }
    }

    public void Complete()
    {
        if (Status != AssignmentStatus.Active && Status != AssignmentStatus.Overdue)
            throw new InvalidOperationException("Only Active or Overdue assignments can be completed");

        if (AssignmentTasks.Any(t => t.Status != AssignmentTaskStatus.Completed))
            throw new InvalidOperationException("Assignment cannot be marked Completed if any task is incomplete");

        Status = AssignmentStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        SetUpdatedAt();
    }
}
