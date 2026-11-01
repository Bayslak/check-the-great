using CheckTheGreat.Domain.Board;
using CheckTheGreat.Domain.Matches;
using CheckTheGreat.Domain.Pieces;

namespace CheckTheGreat.Application.Matches;

/*
var initialState = InitialPositionFactory.CreateStandard();
var rules = new GameRules();

var settings = new MatchSettings(
    WhiteController: ControllerType.Human,
    BlackController: ControllerType.Cpu);

var match = new MatchController(
    initialState,
    rules,
    settings,
    whiteAi: new MinimaxAi(rules, new Evaluator()),
    blackAi: new MinimaxAi(rules, new Evaluator()));
    
var result = match.PlayHumanMove(Side.White, move);
await AnimateMove(result);

var cpuResult = await match.PlayCpuMoveAsync(
    new SearchOptions(MaxDepth: 3),
    cancellationToken);

await AnimateMove(cpuResult);
*/

public static class InitialPositionFactory
{
    public static GameState CreateStandard()
    {
        var board = new Board(8, 8);
        var pieces = new Dictionary<Square, Piece>();
        var id = 1;

        AddSide(Side.White, backRank: 0, pawnRank: 1);
        AddSide(Side.White, backRank: 7, pawnRank: 6);

        return new GameState(board, pieces, Side.White);

        void AddSide(Side side, int backRank, int pawnRank)
        {
            pieces.Add(new Square(0, backRank), new Rook(id++, side));
            pieces.Add(new Square(1, backRank), new Knight(id++, side));
            pieces.Add(new Square(2, backRank), new Bishop(id++, side));
            pieces.Add(new Square(3, backRank), new Queen(id++, side));
            pieces.Add(new Square(4, backRank), new King(id++, side));
            pieces.Add(new Square(5, backRank), new Bishop(id++, side));
            pieces.Add(new Square(6, backRank), new Knight(id++, side));
            pieces.Add(new Square(7, backRank), new Rook(id++, side));

            for (var x = 0; x < 8; x++)
                pieces.Add(new Square(x, pawnRank), new Pawn(id++, side));
        }
    }
}