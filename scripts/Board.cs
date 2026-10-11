using System.Collections.Generic;
using Godot;
namespace ChecktheGreat.scripts;

public partial class Board : Node2D
{
    [Export] private PackedScene _cellPrefab;
    public int Rows, Cols;
    private Dictionary<CellPosition, Cell> _tiles = [];
    
    public Vector2 CellTextureSize = Vector2.Zero;

    public bool InitializeBoard(Level level)
    {
        Rows = level.Rows;
        Cols = level.Columns;
        
        _tiles = new Dictionary<CellPosition, Cell>();
        var emptyTiles = level.EmptyTiles;

        bool lightCell = true;
        for (int i = 1; i <= Cols; i++)
        {
            for (int j = 1; j <= Rows; j++)
            {
                lightCell = !lightCell;
                var cellPosition = new CellPosition(i, j);
                if (emptyTiles.Contains(cellPosition))
                    continue;
                
                var cellToAdd = _cellPrefab.Instantiate<Cell>();

                cellToAdd.InitializeCell(lightCell);
                cellToAdd.Name = $"Cell_{i}_{j}_" + (lightCell ? "Light" : "Dark");

                if (CellTextureSize == Vector2.Zero)
                {
                    CellTextureSize = cellToAdd.GetTextureSize();
                }
                
                _tiles.Add(cellPosition, cellToAdd);
                AddChild(cellToAdd);
                
                cellToAdd.Position = new Vector2(this.Position.X + (CellTextureSize.X) * i, this.Position.Y - (CellTextureSize.Y) * j);
            }
            
            lightCell = !lightCell;
        }

        return true;
    }

    public Cell GetCellByCellPosition(CellPosition position)
    {
        return _tiles[position];
    }
}