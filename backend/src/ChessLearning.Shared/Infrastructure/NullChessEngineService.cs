using ChessLearning.Shared.Application.Contracts;

namespace ChessLearning.Shared.Infrastructure;

public class NullChessEngineService : IChessEngineService
{
    public Task<PositionAnalysis> AnalyzePositionAsync(string fen, AnalysisOptions options, CancellationToken ct = default)
    {
        return Task.FromResult(new PositionAnalysis(fen, 0.0, Array.Empty<string>(), false, null));
    }

    public Task<GameAnalysis> AnalyzeGameAsync(string pgn, AnalysisOptions options, CancellationToken ct = default)
    {
        return Task.FromResult(new GameAnalysis(pgn, new List<MoveAnalysis>(), "Phase 1 - Engine not integrated"));
    }

    public Task<bool> IsHealthyAsync(CancellationToken ct = default)
    {
        return Task.FromResult(true);
    }
}
