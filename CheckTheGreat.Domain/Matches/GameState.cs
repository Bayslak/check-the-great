using CheckTheGreat.Domain.Board;

namespace CheckTheGreat.Domain.Matches;

public sealed class GameState
{
    private readonly Dictionary<Square, Pieces.Piece> _pieces;
    public Board.Board Board { get; }
    public Side SideToMove { get; }
    
    public int MovesCount { get; }
    
    public IEnumerable<KeyValuePair<Square, Pieces.Piece>> Pieces => _pieces;

    public GameState(Board.Board board, 
        IEnumerable<KeyValuePair<Square, Pieces.Piece>> pieces,
        Side sideToMove, 
        int movesCount = 0)
    {
        ArgumentNullException.ThrowIfNull(board);
        ArgumentNullException.ThrowIfNull(pieces);
        ArgumentOutOfRangeException.ThrowIfNegative(movesCount);

        Board = board;
        SideToMove = sideToMove;
        MovesCount = movesCount;
        
        _pieces = pieces.ToDictionary(p => p.Key, p => p.Value);
        
        if (_pieces.Any(p => !board.Contains(p.Key)))
            throw new ArgumentException("Piece not on the board.");
        
        if (_pieces.Values.Select(p => p.Id).Distinct().Count() != _pieces.Count)
            throw new ArgumentException("IDs must be unique.");
    }

    public GameState WithPiece(Square square, Pieces.Piece piece)
    {
        var copy = new Dictionary<Square, Pieces.Piece>(_pieces) { [square] = piece };
        return new GameState(Board, copy, SideToMove, MovesCount);
    }
    
    public Pieces.Piece? PieceAt(Square square) => _pieces.GetValueOrDefault(square);
}