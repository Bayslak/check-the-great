using System.Diagnostics;
using ChecktheGreat.scripts;
using Godot;
using Godot.Collections;

public partial class Board : Node2D
{
    [Export] private PackedScene _cellPrefab;
    [Export] private Dictionary<Cell, CellPosition> _tiles;

    public override void _Ready()
    {
        InitializeBoard(8, 8);
    }

    private void InitializeBoard(int rows, int cols)
    {
        _tiles = new Dictionary<Cell, CellPosition>();

        bool lightCell = true;
        var cellTextureSize = Vector2.Zero;
        for (int i = 1; i <= cols; i++)
        {
            for (int j = 1; j <= rows; j++)
            {
                var cellToAdd = _cellPrefab.Instantiate<Cell>();

                cellToAdd.InitializeCell(lightCell);
                cellToAdd.Name = $"Cell_{i}_{j}_" + (lightCell ? "Light" : "Dark");
                
                if (cellTextureSize == Vector2.Zero)
                    cellTextureSize = cellToAdd.GetTextureSize();
                
                _tiles.Add(cellToAdd, new CellPosition(i, j));
                AddChild(cellToAdd);

                lightCell = !lightCell;
                
                cellToAdd.Position = new Vector2(this.Position.X + (cellTextureSize.X) * i, this.Position.Y + (cellTextureSize.Y) * j);
            }
            
            lightCell = !lightCell;
        }
    }
    
    
}
