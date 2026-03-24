using ChessLearning.Shared.Domain;

namespace ChessLearning.Modules.Content.Domain;

public class Puzzle : BaseEntity
{
    public Guid ContentId { get; set; }
    public string ExternalId { get; set; } = default!;
    public string ExternalSource { get; set; } = "Lichess";
    
    public string FEN { get; set; } = default!;
    public string SolutionMoves { get; set; } = default!; // JSON array of UCI strings
    
    public string? Theme { get; set; }
    public int? Rating { get; set; }
    public DateTime LastSyncedAt { get; set; }

    public Content? Content { get; set; }
}
