using System;
using Godot;

namespace ChecktheGreat.scripts;

public partial class BoardManager : Node2D
{
    [Export] public PieceManager PieceManager;
    
    [Export] private PackedScene _boardPrefab;
    [Export] public Board Board;

    public event Action BoardReady;
    
    private void PositionBoardManager(Level level)
    {
        int boardWidth = level.Columns * 16;
        int boardHeight = level.Rows * 16;
        
        var viewportSize = GetViewport().GetVisibleRect().Size;
        this.Position = new Vector2((viewportSize.X / 2) - (int)(boardWidth / 2), (viewportSize.Y / 2) + (int)(boardHeight / 2));
    }

    public void InitializeBoard(Level level)
    {
        PieceManager.SetBoardManager(this);
        
        PositionBoardManager(level);

        var boardSpawned = _boardPrefab.Instantiate<Board>();
        AddChild(boardSpawned);
        
        Board = boardSpawned;
        _ = Board.InitializeBoard(level);
    }
    
    public Cell GetCellByPosition(CellPosition position)
    {
        return Board.GetCellByCellPosition(position);
    }
}