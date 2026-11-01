using CheckTheGreat.Domain.Board;
using CheckTheGreat.Domain.Matches;
using CheckTheGreat.Domain.Pieces;

namespace CheckTheGreat.AI.Evaluation;

public sealed class Evaluator : IEvaluator
{
    public float Evaluate(GameState state)
    {
        float score = 0;

        foreach (var (square, piece) in state.Pieces)
        {
            var pieceValue = GetPieceValue(piece);
            var positionBonus = GetPositionBonus(state, square, piece);
            var mobility = piece.GetCandidateMoves(state, square).Count * 2;

            var pieceScore = pieceValue + positionBonus + mobility;

            score += piece.Owner == Side.Black
                ? pieceScore
                : -pieceScore;
        }

        return score;
    }

    private static float GetPieceValue(Piece piece) => piece switch
    {
        Pawn => 100,
        Knight => 320,
        Bishop => 330,
        Rook => 500,
        Queen => 900,
        King => 0,
        _ => throw new NotSupportedException($"Valore mancante per {piece.GetType().Name}")
    };

    private static float GetPositionBonus(
        GameState state,
        Square square,
        Piece piece)
    {
        switch (piece)
        {
            case Pawn:
            {
                var advancement = piece.Owner == Side.White
                    ? square.Y - 1
                    : state.Board.Height - 2 - square.Y;

                return Math.Max(0, advancement) * 5;
            }
            case Knight or Bishop:
            {
                var centerX = (state.Board.Width - 1) / 2f;
                var centerY = (state.Board.Height - 1) / 2f;

                var distance =
                    Math.Abs(square.X - centerX) +
                    Math.Abs(square.Y - centerY);

                var maxDistance = centerX + centerY;

                return (maxDistance - distance) * 4;
            }
            default:
                return 0;
        }
    }
}