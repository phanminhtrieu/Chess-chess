namespace ChessLearning.Shared.Application.Contracts;

/// <summary>
/// Abstraction over the chess engine (Stockfish).
/// Phase 1: Use NullChessEngineService (no-op).
/// Phase 2: Replace with StockfishProcessService.
/// </summary>
public interface IChessEngineService
{
    Task<PositionAnalysis> AnalyzePositionAsync(string fen, AnalysisOptions options, CancellationToken ct = default);
    Task<GameAnalysis> AnalyzeGameAsync(string pgn, AnalysisOptions options, CancellationToken ct = default);
    Task<bool> IsHealthyAsync(CancellationToken ct = default);
}

public record AnalysisOptions(int Depth = 20, int MultiPv = 3, int TimeLimitMs = 5000);

public record PositionAnalysis(
    string Fen,
    double Evaluation,
    string[] BestMoves,
    bool IsMate,
    int? MateIn
);

public record GameAnalysis(
    string Pgn,
    List<MoveAnalysis> Moves,
    string Summary
);

public record MoveAnalysis(
    int MoveNumber,
    string Move,
    double EvalBefore,
    double EvalAfter,
    MoveQuality Quality
);

public enum MoveQuality { Best, Good, Inaccuracy, Mistake, Blunder }
