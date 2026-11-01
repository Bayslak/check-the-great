using CheckTheGreat.Domain.Matches;

namespace CheckTheGreat.Application.Matches;

public sealed record MatchSettings(
    ControllerType WhiteController,
    ControllerType BlackController)
{
    public ControllerType ControllerFor(Side side) => side switch
    {
        Side.White => WhiteController,
        Side.Black => BlackController,
        _ => throw new ArgumentOutOfRangeException(nameof(side))
    };
}