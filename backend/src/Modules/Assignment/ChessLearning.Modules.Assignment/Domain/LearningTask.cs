using ChessLearning.Shared.Domain;

namespace ChessLearning.Modules.Assignment.Domain;

public enum ContentType
{
    Puzzle,
    GameReview,
    Custom
}

public class LearningTask : AggregateRoot
{
    public Guid ContentId { get; set; } // Reference to Content module

    public ContentType ContentType { get; set; }
    public string Title { get; set; } = default!;
    public string Instructions { get; set; } = default!;
    
    public string CreatedById { get; set; } = default!;
    public bool IsPublic { get; set; }
}
