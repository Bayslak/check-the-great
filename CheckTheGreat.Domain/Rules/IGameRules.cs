using CheckTheGreat.Domain.Matches;

namespace CheckTheGreat.Domain.Rules;

public interface IGameRules
{
    IReadOnlyList<Move> GetLegalMoves(GameState state);
    MoveResult ApplyMove(GameState state, Move move);
    GameOutcome GetOutcome(GameState state);
}