using CheckTheGreat.Domain.Board;
using CheckTheGreat.Domain.Matches;

namespace CheckTheGreat.Domain.Pieces;

public abstract record Piece(int Id, Side Owner)
{
    public abstract PieceType GetPieceType();
    public abstract IReadOnlyList<Move> GetCandidateMoves(GameState state, Square fromSq);
}