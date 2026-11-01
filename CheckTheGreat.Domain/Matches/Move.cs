using CheckTheGreat.Domain.Board;

namespace CheckTheGreat.Domain.Matches;

public enum MoveMode
{
    MoveOnly,
    CaptureOnly,
    MoveOrCapture
}

public readonly record struct Move(Square From, Square To, MoveMode Mode);