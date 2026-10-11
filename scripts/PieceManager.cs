using System.Collections.Generic;
using Godot;

namespace ChecktheGreat.scripts;

public partial class PieceManager : Node
{
    [Export] private PackedScene _piecePrefab;
    public Dictionary<CellPosition, Piece> Pieces = [];
    
    [Export] public BoardManager BoardManager;

    public void SetBoardManager(BoardManager boardManager)
    {
        BoardManager = boardManager;
        BoardManager.BoardReady += HandleBoardReadyEvent;
    }

    public override void _ExitTree()
    {
        if (BoardManager != null)
        {
            BoardManager.BoardReady -= HandleBoardReadyEvent;
        }
    }

    private void HandleBoardReadyEvent()
    {
    }

    public void SpawnPawn(CellPosition startingPosition)
    {
        var pawnSpawned = _piecePrefab.Instantiate<Piece>();
        AddChild(pawnSpawned);
        
        var cell = BoardManager.GetCellByPosition(startingPosition);
        
        pawnSpawned.Position = cell.GlobalPosition;
    }
}