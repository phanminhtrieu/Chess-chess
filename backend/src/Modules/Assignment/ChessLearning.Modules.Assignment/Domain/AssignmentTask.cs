using ChessLearning.Shared.Domain;

namespace ChessLearning.Modules.Assignment.Domain;

public enum AssignmentTaskStatus
{
    Pending,
    Completed
}

public class AssignmentTask : BaseEntity
{
    public Guid AssignmentId { get; set; }
    public Guid TaskId { get; set; }    // Points to Task entity.
    
    public int Order { get; set; }
    public AssignmentTaskStatus Status { get; private set; } = AssignmentTaskStatus.Pending;
    public DateTime? CompletedAt { get; private set; }

    public Assignment? Assignment { get; set; }
    public LearningTask? Task { get; set; }

    public void MarkComplete()
    {
        if (Status == AssignmentTaskStatus.Completed) return;
        Status = AssignmentTaskStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        SetUpdatedAt();
    }
}
