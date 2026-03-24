using ChessLearning.Shared.Domain;

namespace ChessLearning.Modules.Game.Domain;

public enum GameSource
{
    Chesscom,
    Lichess
}

public class ImportedGame : AggregateRoot
{
    public Guid UserId { get; set; }
    public Guid? TenantId { get; set; }
    
    public GameSource Source { get; set; }
    public string ExternalId { get; set; } = default!; // Unique per source + user
    
    public string PGN { get; set; } = default!;
    public string White { get; set; } = default!;
    public string Black { get; set; } = default!;
    public string Result { get; set; } = default!;
    
    public DateTime PlayedAt { get; set; }
    public DateTime ImportedAt { get; set; } = DateTime.UtcNow;
    
    public Guid? LinkedTaskId { get; set; } // Points to Assignment.Task if optional linked
}
