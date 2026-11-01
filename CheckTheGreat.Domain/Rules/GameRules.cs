using CheckTheGreat.Domain.Matches;
using CheckTheGreat.Domain.Pieces;

namespace CheckTheGreat.Domain.Rules;

public sealed class GameRules : IGameRules
{
    private int MaxMovesCount { get; }

    public GameRules(int maxMovesCount = 200)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maxMovesCount, 1);

        MaxMovesCount = maxMovesCount;
    }

    public IReadOnlyList<Move> GetLegalMoves(GameState state)
    {
        if (BasicOutcome(state) != GameOutcome.Ongoing)
        {
            return [];
        }

        return state.Pieces
            .Where(entry => entry.Value.Owner == state.SideToMove)
            .SelectMany(entry => entry.Value.GetCandidateMoves(state, entry.Key))
            .Where(move => SatisfiesGlobalRules(state, move))
            .ToArray();
    }

    private bool SatisfiesGlobalRules(GameState state, Move move)
    {
        return true;
    }

    public MoveResult ApplyMove(GameState state, Move move)
    {
        if (!GetLegalMoves(state).Contains(move))
        {
            throw new ArgumentException("Move not legal.", nameof(move));
        }

        var movingPiece = state.PieceAt(move.From) ?? throw new InvalidOperationException("No piece at the starting square.");

        var capturedPiece = state.PieceAt(move.To);

        var pieces = state.Pieces.ToDictionary(p => p.Key, p => p.Value);

        pieces.Remove(move.From);
        pieces[move.To] = movingPiece;

        var next = state.SideToMove.Opponent();

        var stateAfter = new GameState(
            state.Board,
            pieces,
            next,
            state.MovesCount + 1);

        return new MoveResult(
            StateAfter: stateAfter,
            Move: move,
            MovingPiece: movingPiece,
            CapturedPiece: capturedPiece);
    }

    public GameOutcome GetOutcome(GameState state)
    {
        var basic = BasicOutcome(state);
        if (basic != GameOutcome.Ongoing) return basic;
        return GetLegalMoves(state).Count == 0 ? GameOutcome.Draw : GameOutcome.Ongoing;
    }

    private GameOutcome BasicOutcome(GameState state)
    {
        var whiteKing = state.Pieces.Any(p => p.Value is King { Owner: Side.White });
        var blackKing = state.Pieces.Any(p => p.Value is King { Owner: Side.Black });

        switch (whiteKing)
        {
            case false when !blackKing:
                return GameOutcome.Draw;
            case false:
                return GameOutcome.ComputerWin;
        }

        if (!blackKing)
        {
            return GameOutcome.HumanWin;
        }

        return state.MovesCount >= MaxMovesCount ? GameOutcome.Draw : GameOutcome.Ongoing;
    }
}