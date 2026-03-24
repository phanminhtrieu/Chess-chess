using ChessLearning.Shared.Domain;

namespace ChessLearning.Modules.Game.Domain;

public enum GameImportJobStatus
{
    Pending,
    Running,
    Completed,
    Failed
}

public class GameImportJob : AggregateRoot
{
    public Guid UserId { get; set; }
    public GameSource Source { get; set; }
    
    public GameImportJobStatus Status { get; set; } = GameImportJobStatus.Pending;
    public bool IsAutoSync { get; set; }
    
    public DateTime ScheduledAt { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    public int GamesImported { get; set; }
    public string? ErrorMessage { get; set; }
}
