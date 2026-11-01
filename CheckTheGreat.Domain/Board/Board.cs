namespace CheckTheGreat.Domain.Board;

public sealed class Board
{
    private readonly HashSet<Square> _cells;
    public int Width { get; }
    public int Height { get; }

    public Board(int width, int height, IEnumerable<Square>? holes = null)
    {
        if (width <= 0 || height <= 0) throw new ArgumentOutOfRangeException();
        Width = width;
        Height = height;
        _cells = [];
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                _cells.Add(new Square(x, y));
            }
        }
        
        if (holes is null) return;
        
        foreach (var hole in holes)
        {
            _cells.Remove(hole);
        }
    }

    public bool Contains(Square square) => _cells.Contains(square);
}