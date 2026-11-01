using CheckTheGreat.AI.Search;
using CheckTheGreat.Domain.Matches;
using CheckTheGreat.Domain.Rules;

namespace CheckTheGreat.Application.Matches;

public sealed class MatchController(
    GameState initialState,
    IGameRules rules,
    MatchSettings settings,
    MinimaxAi whiteAi,
    MinimaxAi blackAi)
{
    private GameState State { get; set; } = initialState;
    private GameOutcome Outcome => rules.GetOutcome(State);
    public bool IsThinking { get; private set; }

    public MoveResult PlayHumanMove(Side side, Move move)
    {
        EnsureOngoing();

        if (side != State.SideToMove)
            throw new InvalidOperationException("Wrong side turn.");

        if (settings.ControllerFor(side) != ControllerType.Human)
            throw new InvalidOperationException("This side is controlled by CPU.");

        // ApplyMove verifica anche che la mossa sia legale.
        return Apply(move);
    }

    public async Task<MoveResult> PlayCpuMoveAsync(
        SearchOptions options,
        CancellationToken cancellationToken = default)
    {
        EnsureOngoing();

        if (IsThinking)
            throw new InvalidOperationException("A search is already in progress.");

        var side = State.SideToMove;

        if (settings.ControllerFor(side) != ControllerType.Cpu)
            throw new InvalidOperationException("Questo lato è controllato dall'umano.");

        var ai = side == Side.White ? whiteAi : blackAi;
        var snapshot = State;

        IsThinking = true;

        try
        {
            var decision = await Task.Run(
                () => ai.ChooseMove(snapshot, options, cancellationToken),
                cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            if (!ReferenceEquals(State, snapshot))
                throw new InvalidOperationException(
                    "Game changed during search.");

            if (decision.Move is not { } move)
                throw new InvalidOperationException(
                    "AI didn't return any move.");

            return Apply(move);
        }
        finally
        {
            IsThinking = false;
        }
    }

    private MoveResult Apply(Move move)
    {
        var result = rules.ApplyMove(State, move);
        State = result.StateAfter;
        return result;
    }

    private void EnsureOngoing()
    {
        if (Outcome != GameOutcome.Ongoing)
            throw new InvalidOperationException("Game is not ongoing.");
    }
}