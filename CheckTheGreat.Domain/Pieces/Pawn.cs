using CheckTheGreat.Domain.Board;
using CheckTheGreat.Domain.Matches;

namespace CheckTheGreat.Domain.Pieces;

public sealed record Pawn(int Id, Side Owner, bool CanCaptureSideways = false) : Piece(Id, Owner)
{

    public override PieceType GetPieceType() => PieceType.Pawn;

    public override IReadOnlyList<Move> GetCandidateMoves(GameState state, Square fromSq)
    {
        var dy = (int)Owner * 1;

        var moves = new List<Move>
        {
            new(fromSq, fromSq with { Y = fromSq.Y + dy }, MoveMode.MoveOnly),
            
            new(fromSq, new Square(fromSq.X-1, fromSq.Y + dy), MoveMode.CaptureOnly),
            new(fromSq, new Square(fromSq.X+1, fromSq.Y + dy), MoveMode.CaptureOnly),
        };
        
        var candidates = moves.Where(m =>
        {
            if (!state.Board.Contains(m.To)) return false;
            
            var target = state.PieceAt(m.To);
            if (target?.Owner == state.SideToMove) return false;
            return m.Mode switch
            {
                MoveMode.CaptureOnly when target is null => false,
                MoveMode.MoveOnly when target is not null => false,
                _ => true
            };
        }).ToList();
        
        return candidates;
    }
}