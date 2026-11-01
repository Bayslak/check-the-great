namespace CheckTheGreat.Domain.Matches;

public static class SideExtensions
{
    public static Side Opponent(this Side player) => player switch
    {
        Side.White => Side.Black,
        Side.Black => Side.White,
        _ => throw new ArgumentOutOfRangeException(nameof(player))
    };
}