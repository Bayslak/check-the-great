using CheckTheGreat.Domain.Board;
using CheckTheGreat.Domain.Matches;

namespace CheckTheGreat.Domain.Pieces;

public sealed record Knight(int Id, Side Owner) : Piece(Id, Owner)
{
    private static readonly (int X, int Y)[] KnightOffsets =
    {
        (1, 2), (2, 1), (2, -1), (1, -2),
        (-1, -2), (-2, -1), (-2, 1), (-1, 2)
    };

    public override PieceType GetPieceType() => PieceType.Knight;

    public override IReadOnlyList<Move> GetCandidateMoves(GameState state, Square fromSq)
    {
        var moves= KnightOffsets.Select(offset =>
                new Move(fromSq, new Square(fromSq.X + offset.X, fromSq.Y + offset.Y), MoveMode.MoveOrCapture)
            ).ToList();
        
        var candidates = moves.Where(m =>
        {
            if (!state.Board.Contains(m.To)) return false;
            
            var target = state.PieceAt(m.To);
            return target?.Owner != state.SideToMove;
        }).ToList();

        return candidates;
    }
}