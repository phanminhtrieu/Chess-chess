using ChessLearning.Shared.Domain;

namespace ChessLearning.Modules.Learning.Domain;

public enum LearningPlanStatus
{
    Active,
    Paused,
    Completed,
    Archived
}

public class LearningPlan : AggregateRoot
{
    public Guid UserId { get; set; }
    public Guid? TenantId { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public LearningPlanStatus Status { get; set; } = LearningPlanStatus.Active;

    public ICollection<DailyGoal> DailyGoals { get; set; } = new List<DailyGoal>();
}
