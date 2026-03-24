using ChessLearning.Shared.Domain;

namespace ChessLearning.Modules.Content.Domain;

public class PuzzleAttempt : AggregateRoot
{
    public Guid UserId { get; set; }
    public Guid PuzzleId { get; set; }
    
    public bool IsSuccess { get; set; }
    public int TimeSpentSeconds { get; set; }
    public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;
}
