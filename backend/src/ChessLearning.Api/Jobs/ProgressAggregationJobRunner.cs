namespace ChessLearning.Api.Jobs;

public class ProgressAggregationJobRunner
{
    public Task ExecuteAsync()
    {
        // TODO: Query all ProgressEvents from yesterday
        // Recalculate Streaks, totals, and puzzle accuracy
        // Upsert into ProgressSummary table
        return Task.CompletedTask;
    }
}
