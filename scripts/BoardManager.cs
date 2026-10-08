using System;
using Godot;

namespace ChecktheGreat.scripts;

public partial class BoardManager : Node2D
{
    [Export] public PieceManager PieceManager;
    
    [Export] private PackedScene _boardPrefab;
    [Export] public Board Board;

    public event Action BoardReady;
    
    public override void _Ready()
    {
        PieceManager.SetBoardManager(this);
        
        var viewportSize = GetViewport().GetVisibleRect().Size;
        this.Position = new Vector2(viewportSize.X / 2, viewportSize.Y / 2);
        
        var boardSpawned = _boardPrefab.Instantiate<Board>();
        AddChild(boardSpawned);
        
        Board = boardSpawned;
        bool boardInitialized = Board.InitializeBoard(8, 8);

        if (boardInitialized)
        {
            BoardReady?.Invoke();
        }
    }

    public Cell GetCellByPosition(CellPosition position)
    {
        return Board.GetCellByCellPosition(position);
    }
}