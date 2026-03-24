using ChessLearning.Shared.Domain;

namespace ChessLearning.Modules.Progress.Domain;

public class ProgressSummary : BaseEntity
{
    public Guid UserId { get; set; }
    
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    
    public int TotalPuzzlesSolved { get; set; }
    public double PuzzleAccuracy { get; set; } // Percentage
    
    public int TotalTasksCompleted { get; set; }
    public int TotalGamesImported { get; set; }
    
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    
    public int TotalTimeSpentMinutes { get; set; }
}
