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
    public Guid TeacherId { get; set; }
    public Guid StudentId { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? LearningPlanId { get; set; }

    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateTime? DueDate { get; set; }
    public AssignmentStatus Status { get; set; } = AssignmentStatus.Draft;

    public ICollection<AssignmentTask> AssignmentTasks { get; set; } = new List<AssignmentTask>();
}
