namespace ChessLearning.Api.Jobs;

public class GameImportJobRunner
{
    public Task ExecuteAsync(Guid userId, string source, string username)
    {
        // TODO: Call Chess.com / Lichess API
        // Download games, parse PGN, save to Database
        // Publish GameImported event
        return Task.CompletedTask;
    }
}
