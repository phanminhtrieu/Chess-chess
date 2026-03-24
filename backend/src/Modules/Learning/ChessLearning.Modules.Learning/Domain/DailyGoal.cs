using ChessLearning.Shared.Domain;

namespace ChessLearning.Modules.Learning.Domain;

public class DailyGoal : BaseEntity
{
    public Guid LearningPlanId { get; set; }
    public DateTime Date { get; set; }
    public int TargetMinutes { get; set; }
    public int TargetTaskCount { get; set; }
    public bool IsCompleted { get; set; }

    public LearningPlan? LearningPlan { get; set; }
}
