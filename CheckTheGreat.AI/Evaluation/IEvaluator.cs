using CheckTheGreat.Domain.Matches;

namespace CheckTheGreat.AI.Evaluation;

public interface IEvaluator
{
    /// <summary>
    /// Evaluates a non-conclusive position.
    /// Positive: black has advantage.
    /// Negative: white has advantage.
    /// </summary>
    float Evaluate(GameState state);
}