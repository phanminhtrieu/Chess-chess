using ChessLearning.Shared.Domain;

namespace ChessLearning.Modules.Progress.Domain;

public class ProgressEvent : AggregateRoot
{
    public Guid UserId { get; set; }
    public Guid? TenantId { get; set; }
    
    // e.g. "PuzzleSolved", "TaskCompleted", "GameImported"
    public string Type { get; set; } = default!; 
    
    public double Value { get; set; } // e.g. 1 for completed task, % for accuracy
    public string? Metadata { get; set; } // JSON with specific context (e.g. { "taskId": "..." })
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
