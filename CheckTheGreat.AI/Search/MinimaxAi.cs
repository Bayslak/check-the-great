using CheckTheGreat.AI.Evaluation;
using CheckTheGreat.Domain.Matches;
using CheckTheGreat.Domain.Rules;

namespace CheckTheGreat.AI.Search;

public sealed class MinimaxAi(IGameRules rules, IEvaluator evaluator)
{
    // Nessuno stato di ricerca condiviso: ogni chiamata usa il proprio contesto.
    // CancellationToken interrompe con OperationCanceledException.
    // MaxNodes esaurito restituisce l'ultima profondità completata.
    public SearchResult ChooseMove(GameState state, SearchOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= new SearchOptions();
        if (options.MaxDepth < 1 || options.MaxDepth > 64 || options.MaxNodes < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(options));
        }

        cancellationToken.ThrowIfCancellationRequested();

        var outcome = rules.GetOutcome(state);
        if (outcome != GameOutcome.Ongoing)
        {
            return new SearchResult(null, TerminalScore(outcome), 0, 0);
        }

        var moves = rules.GetLegalMoves(state);

        if (moves.Count == 0)
        {
            throw new InvalidOperationException("Esito e mosse incoerenti.");
        }

        var search = new SearchContext(rules, evaluator, options, cancellationToken);

        // Fallback move with not calculated score
        var best = new SearchResult(moves[0], float.NaN, 0, 0);

        for (var depth = 1; depth <= options.MaxDepth; depth++)
        {
            try
            {
                var (move, score) = search.Root(state, moves, depth);
                best = new SearchResult(move, score, depth, search.Nodes);
            }
            catch (NodeBudgetExceededException)
            {
                break;
            }
        }

        return best with { VisitedNodes = search.Nodes };
    }

    private static float TerminalScore(GameOutcome gameOutcome) => gameOutcome switch
    {
        GameOutcome.ComputerWin => float.PositiveInfinity,
        GameOutcome.HumanWin => float.NegativeInfinity,
        GameOutcome.Draw => 0,
        _ => throw new ArgumentException("La partita non è finita.")
    };

    private sealed class NodeBudgetExceededException : Exception
    {
    }

    private sealed class SearchContext(IGameRules rules, IEvaluator evaluator, SearchOptions options, CancellationToken token)
    {
        public long Nodes { get; private set; }

        public (Move Move, float Score) Root(GameState state, IReadOnlyList<Move> moves, int depth)
        {
            var blackPlaying = state.SideToMove == Side.Black;
            var best = blackPlaying ? float.NegativeInfinity : float.PositiveInfinity;

            var blackLowerLimit = float.NegativeInfinity;
            var whiteUpperLimit = float.PositiveInfinity;

            var chosen = moves[0];
            var first = true;

            foreach (var move in moves)
            {
                var moveResult = rules.ApplyMove(state, move);
                var score = Search(moveResult.StateAfter, depth - 1, blackLowerLimit, whiteUpperLimit);

                if (first || (blackPlaying ? score > best : score < best))
                {
                    best = score;
                    chosen = move;
                    first = false;
                }

                if (blackPlaying)
                {
                    blackLowerLimit = Math.Max(blackLowerLimit, best);
                }
                else
                {
                    whiteUpperLimit = Math.Min(whiteUpperLimit, best);
                }

                if (blackLowerLimit >= whiteUpperLimit)
                {
                    break;
                }
            }

            return (chosen, best);
        }

        private float Search(GameState state, int depth, float cpuLowerLimit, float playerUpperLimit)
        {
            token.ThrowIfCancellationRequested();
            if (Nodes >= options.MaxNodes)
            {
                throw new NodeBudgetExceededException();
            }

            Nodes++;

            var outcome = rules.GetOutcome(state);
            if (outcome != GameOutcome.Ongoing)
            {
                return TerminalScore(outcome);
            }

            if (depth == 0)
            {
                var value = evaluator.Evaluate(state);
                if (float.IsNaN(value) || float.IsInfinity(value))
                {
                    throw new InvalidOperationException("L'evaluator deve restituire un valore finito.");
                }

                return value;
            }

            var maximize = state.SideToMove == Side.Black;
            var best = maximize ? float.NegativeInfinity : float.PositiveInfinity;

            foreach (var move in rules.GetLegalMoves(state))
            {
                var moveResult = rules.ApplyMove(state, move);
                var value = Search(moveResult.StateAfter, depth - 1, cpuLowerLimit, playerUpperLimit);
                best = maximize ? Math.Max(best, value) : Math.Min(best, value);
                if (maximize) cpuLowerLimit = Math.Max(cpuLowerLimit, best);
                else playerUpperLimit = Math.Min(playerUpperLimit, best);
                if (cpuLowerLimit >= playerUpperLimit) break;
            }

            return best;
        }
    }
}