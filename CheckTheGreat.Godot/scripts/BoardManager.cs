using System;
using System.Collections.Generic;
using Godot;

namespace CheckTheGreat.Godot.scripts;

public partial class BoardManager : Node2D
{
    [Export] public PieceManager PieceManager;
    
    [Export] private PackedScene _boardPrefab;
    [Export] public Board Board;

    public event Action BoardReady;

    public Cell? HoveredCell;
    
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
        _ = Board.InitializeBoard(level, this);
        
        BoardReady?.Invoke();
    }
    
    public Cell GetCellByPosition(CellPosition position)
    {
        return Board.GetCellByCellPosition(position);
    }

    public CellPosition GetCellPositionByCell(Cell cell)
    {
        return Board.GetCellPositionByCell(cell);
    }

    public void SetHoveredCell(Cell? cell)
    {
        HoveredCell = cell;

        if (cell != null)
            GD.Print("Hovered cell: " + cell.Name);
    }

    public void HighlightStartingTiles(HashSet<CellPosition> currentLevelStartingTiles)
    {
        foreach (var kvp in Board.Tiles)
        {
            if (currentLevelStartingTiles.Contains(kvp.Key))
                Board.Tiles[kvp.Key].SetStartingTile(true);
        }
    }

    public void DeHighlightTiles()
    {
        foreach (var kvp in Board.Tiles)
        {
            var cell = kvp.Value;
            if (cell.IsStartingTile)
                cell.SetStartingTile(false);
        }
    }
}