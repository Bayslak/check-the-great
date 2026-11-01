using CheckTheGreat.Domain.Board;
using CheckTheGreat.Domain.Matches;

namespace CheckTheGreat.Domain.Pieces;

public sealed record Rook(int Id, Side Owner) : Piece(Id, Owner)
{
    private static readonly (int x, int y)[] Directions =
    [
        (0, -1), (1, 0),
        (0, 1), (-1, 0)
    ];

    public override PieceType GetPieceType() => PieceType.Rook;

    public override IReadOnlyList<Move> GetCandidateMoves(
        GameState state,
        Square fromSq)
    {
        var moves = new List<Move>();

        foreach (var (dx, dy) in Directions)
        {
            var target = new Square(fromSq.X + dx, fromSq.Y + dy);

            while (state.Board.Contains(target))
            {
                var occupant = state.PieceAt(target);

                if (occupant?.Owner == Owner)
                    break;

                moves.Add(new Move(fromSq, target, MoveMode.MoveOrCapture));

                if (occupant is not null)
                    break;

                target = new Square(target.X + dx, target.Y + dy);
            }
        }

        return moves;
    }
}