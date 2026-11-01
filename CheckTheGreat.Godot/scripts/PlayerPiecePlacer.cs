using System.Collections.Generic;
using Godot;

namespace CheckTheGreat.Godot.scripts;

public partial class PlayerPiecePlacer : Node2D
{
    [Export] private GameManager _gameManager;
    [Export] private BoardManager _boardManager;
    [Export] private PieceManager _pieceManager;
    
    [Export] public bool Enabled;
    [Export] public int NumberOfPiecesToPlace;
    private int _numberOfPiecesPlaced;
    
    private HashSet<CellPosition> _startingTiles;

    public override void _Input(InputEvent @event)
    {
        if(!Enabled) return;
        
        base._Input(@event);

        if (@event is not InputEventMouseButton mouseEvent)
            return;

        if (mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed && !mouseEvent.IsEcho()) // study IsEcho()
        {
            PlacePiece();
            //@event.Dispose();

            if (NumberOfPiecesToPlace == _numberOfPiecesPlaced)
                Disable();
        }
    }

    private void PlacePiece()
    {
        if (_boardManager.HoveredCell == null)
        {
            GD.PrintErr("Wtf you doing bro?");
            return;
        }
        
        var position = _boardManager.GetCellPositionByCell(_boardManager.HoveredCell);

        if (!_startingTiles.Contains(position))
        {
            GD.PrintErr("You can't here bro.");
            return;
        }
        
        _pieceManager.SpawnPawn(position, PieceType.King, PieceHouse.Light);
        
        _numberOfPiecesPlaced++;
    }

    public void Enable(int numberOfPiecesToPlace, HashSet<CellPosition> startingTiles)
    {
        Enabled = true;
        _startingTiles = startingTiles;
        
        NumberOfPiecesToPlace = numberOfPiecesToPlace;
        _numberOfPiecesPlaced = 0;
    }

    public override void _Process(double d)
    {
        if (Enabled && _numberOfPiecesPlaced == NumberOfPiecesToPlace)
            Disable();
    }
    
    private void Disable()
    {
        Enabled = false;
        
        // I dont like calling GameManager methods
        _gameManager.PlayerPiecePlacerDisabled();
        _startingTiles = null;
    }
}