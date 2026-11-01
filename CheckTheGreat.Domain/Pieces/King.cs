using CheckTheGreat.Domain.Board;
using CheckTheGreat.Domain.Matches;

namespace CheckTheGreat.Domain.Pieces;

public sealed record King(int Id, Side Owner) : Piece(Id, Owner)
{
    public override IReadOnlyList<Move> GetCandidateMoves(GameState state, Square fromSq)
    {
        var moves = new List<Move>();

        for (var dx = -1; dx <= 1; dx++)
        for (var dy = -1; dy <= 1; dy++)
            if (dx != 0 || dy != 0)
                moves.Add(new Move(fromSq, new Square(fromSq.X + dx, fromSq.Y + dy), MoveMode.MoveOrCapture));

        var candidates = moves.Where(m =>
        {
            if (!state.Board.Contains(m.To)) return false;

            var target = state.PieceAt(m.To);
            return target?.Owner != state.SideToMove;
        }).ToList();

        return candidates;
    }
}