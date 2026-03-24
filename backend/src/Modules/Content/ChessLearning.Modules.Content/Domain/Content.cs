using ChessLearning.Shared.Domain;

namespace ChessLearning.Modules.Content.Domain;

public enum ContentSource
{
    User,
    System,
    External
}

public enum ContentType
{
    Puzzle,
    GameReview,
    Custom
}

public class Content : AggregateRoot
{
    public ContentType Type { get; set; }
    public string Title { get; set; } = default!;
    public string Body { get; set; } = default!; // JSON or text depending on Type
    
    public ContentSource Source { get; set; }
    public Guid? CreatedById { get; set; } // Nullable for system content
    public Guid? TenantId { get; set; }
    
    // Virtual nav to Puzzle if Type == Puzzle
    public Puzzle? Puzzle { get; set; }
}
