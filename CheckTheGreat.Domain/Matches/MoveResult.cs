using CheckTheGreat.Domain.Pieces;

namespace CheckTheGreat.Domain.Matches;

public sealed record MoveResult(
    GameState StateAfter,
    Move Move,
    Piece MovingPiece,
    Piece? CapturedPiece);