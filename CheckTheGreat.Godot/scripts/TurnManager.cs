using System.Collections.Generic;
using CheckTheGreat.AI.Search;
using CheckTheGreat.Domain.Board;
using CheckTheGreat.Domain.Matches;
using Godot;
using Side = CheckTheGreat.Domain.Matches.Side;

namespace CheckTheGreat.Godot.scripts;

public partial class TurnManager : Node2D
{
    [Export] public bool Enabled { get; set; }
    [Export] public Turn Turn { get; set; } = Turn.None;

    private GameManager _gameManager;
    private BoardManager _boardManager;
    private PieceManager _pieceManager;

    private Piece _selectedPiece;
    private CellPosition _selectedPieceStartingPosition;
    private Dictionary<CellPosition, Move> _possibleMoves = [];

    public override void _Input(InputEvent @event)
    {
        if(!Enabled || Turn != Turn.Human) return;
        
        base._Input(@event);
        
        if(@event is not InputEventMouseButton mouseEvent)
            return;

        if (mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed && !mouseEvent.IsEcho())
            HandlePlayerTurnClick();
    }

    private void HandlePlayerTurnClick()
    {
        if (_boardManager.HoveredCell == null)
        {
            GD.PrintErr("Wtf you doing bro?");
            return;
        }
        
        var position = _boardManager.GetCellPositionByCell(_boardManager.HoveredCell);
        _pieceManager.Pieces.TryGetValue(position, out var piece);
        
        // rethink, rethink, rethink
        if (piece != null && piece.House != PieceHouse.Light)
        {
            GD.PrintErr("Not your piece bro");
            return;
        }

        if (piece != null)
        {
            _boardManager.UnsetPossibleMoves();
            _possibleMoves.Clear();
            
            _selectedPiece = piece;
            _selectedPieceStartingPosition = position;
            
            var candidateMoves = _gameManager.Match.GetCandidateMoves(_gameManager.Match.GetState(),
                new Square(_selectedPieceStartingPosition.Col - 1, _selectedPieceStartingPosition.Row - 1),
                int.Parse(_selectedPiece.Id));
            
            //GD.Print(candidateMoves);

            var possibleCellPositions = new HashSet<CellPosition>();
            foreach (var move in candidateMoves)
            {
                var cellPosition = new CellPosition(move.To.X + 1, move.To.Y + 1);
                _possibleMoves.Add(cellPosition, move);
                possibleCellPositions.Add(cellPosition);
            }
            
            _boardManager.HighlightPossibleMoves(possibleCellPositions);
        }
        else
        {
            if (_selectedPiece != null)
            {
                _possibleMoves.TryGetValue(position, out var move);

                if (move == default) // ti odio rotty?
                    return;

                var moveResult = _gameManager.Match.PlayHumanMove(Side.White, move);
                _gameManager.UseMoveResult(moveResult);
                _boardManager.UnsetPossibleMoves();
            }
        }
    }

    public void Enable(GameManager gameManager, BoardManager boardManager, PieceManager pieceManager)
    {
        GD.Print("TurnManager Enabled");
        Enabled = true;
        
        Turn = Turn.Human;
        
        _gameManager = gameManager;
        _boardManager = boardManager;
        _pieceManager = pieceManager;
    }

    public void Disable()
    {
        GD.Print("TurnManager Disabled");
        Enabled = false;
    }

    public void AdvanceTurn()
    {
        _possibleMoves.Clear();
        _boardManager.UnsetPossibleMoves();
        
        switch (Turn)
        {
            case Turn.Human:
                Turn = Turn.Cpu;
                PlayCpuTurn();
                break;
            case Turn.Cpu:
                Turn = Turn.Human;
                break;
            default:
                GD.PrintErr($"Yo bro, this is weird.. Turn: {Turn.ToString()}");
                break;
        }
    }

    private async void PlayCpuTurn()
    {
        var moveResult = await _gameManager.Match.PlayCpuMoveAsync(new SearchOptions(MaxDepth: 3));
        _gameManager.UseMoveResult(moveResult);
    }
}

public enum Turn { None, Human, Cpu }