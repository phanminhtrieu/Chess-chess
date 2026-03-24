namespace ChessLearning.Api.Jobs;

public class PuzzleSyncJobRunner
{
    public Task ExecuteAsync()
    {
        // TODO: Call Lichess Puzzle Database API
        // Parse CSV or JSON, download latest puzzles
        // Deduplicate and save to Database
        return Task.CompletedTask;
    }
}
