using ChessLearning.Shared.Domain;

namespace ChessLearning.Modules.Assignment.Domain;

public enum AssignmentTaskStatus
{
    Pending,
    Completed,
    Skipped
}

public class AssignmentTask : BaseEntity
{
    public Guid AssignmentId { get; set; }
    public Guid TaskId { get; set; }    // Points to Task entity.
    
    public int Order { get; set; }
    public AssignmentTaskStatus Status { get; set; } = AssignmentTaskStatus.Pending;
    public DateTime? CompletedAt { get; set; }

    public Assignment? Assignment { get; set; }
    public Task? Task { get; set; }
}
