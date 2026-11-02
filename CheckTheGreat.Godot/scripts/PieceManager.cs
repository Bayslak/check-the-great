using System;
using System.Collections.Generic;
using Godot;

namespace CheckTheGreat.Godot.scripts;

public partial class PieceManager : Node
{
    [Export] private PackedScene _piecePrefab;
    [Export] private Texture2D _blackPawn, _blackKnight, _blackBishop, _blackRook, _blackQueen, _blackKing;
    [Export] private Texture2D _whitePawn, _whiteKnight, _whiteBishop, _whiteRook, _whiteQueen, _whiteKing;
    public readonly Dictionary<CellPosition, Piece> Pieces = [];

    [Export] public BoardManager BoardManager;

    public event Action PiecesInitialized;
    
    public void SetBoardManager(BoardManager boardManager)
    {
        BoardManager = boardManager;
    }

    public void SpawnPawn(string id, CellPosition startingPosition, PieceType pieceType, PieceHouse pieceHouse)
    {
        var pawnSpawned = _piecePrefab.Instantiate<Piece>();
        pawnSpawned.Id = id;
        
        AddChild(pawnSpawned);

        var texture2d = GuessTexture(pieceType, pieceHouse);
        pawnSpawned.SetSprite(texture2d);
        pawnSpawned.SetHouse(pieceHouse);
        
        var cell = BoardManager.GetCellByPosition(startingPosition);
        
        pawnSpawned.Position = cell.GlobalPosition;
        Pieces.Add(startingPosition, pawnSpawned);
    }

    private Texture2D GuessTexture(PieceType pieceType, PieceHouse pieceHouse)
    {
        switch (pieceType)
        {
            case PieceType.Pawn:
                switch (pieceHouse)
                {
                    case PieceHouse.Dark:
                        return _blackPawn;
                    case PieceHouse.Light:
                        return _whitePawn;
                }
                break;
            case PieceType.Bishop:
                switch (pieceHouse)
                {
                    case PieceHouse.Dark:
                        return _blackBishop;
                    case PieceHouse.Light:
                        return _whiteBishop;
                }
                break;
            case PieceType.Knight:
                switch (pieceHouse)
                {
                    case PieceHouse.Dark:
                        return _blackKnight;
                    case PieceHouse.Light:
                        return _whiteKnight;
                }
                break;
            case PieceType.Rook:
                switch (pieceHouse)
                {
                    case PieceHouse.Dark:
                        return _blackRook;
                    case PieceHouse.Light:
                        return _whiteRook;
                }
                break;
            case PieceType.Queen:
                switch (pieceHouse)
                {
                    case PieceHouse.Dark:
                        return _blackQueen;
                    case PieceHouse.Light:
                        return _whiteQueen;
                }
                break;
            case PieceType.King:
                switch (pieceHouse)
                {
                    case PieceHouse.Dark:
                        return _blackKing;
                    case PieceHouse.Light:
                        return _whiteKing;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(pieceType), pieceType, null);
        }
        return null;
    }

    public void InitializePieces(Level level)
    {
        foreach (var enemyInitialization in level.Enemies)
        {
            SpawnPawn(enemyInitialization.Id, enemyInitialization.StartingPosition, enemyInitialization.PieceType, PieceHouse.Dark);
        }

        // To rethink when outside simple chess
        foreach (var friendlyInitialization in level.Friendlies)
        {
            SpawnPawn(friendlyInitialization.Id, friendlyInitialization.StartingPosition, friendlyInitialization.PieceType, PieceHouse.Light);
        }
        
        PiecesInitialized?.Invoke();
    }

    public void MovePiece(CellPosition fromPosition, CellPosition toPosition)
    {
        var cellToMoveTo = BoardManager.GetCellByPosition(toPosition);
        var pieceMoving = Pieces[fromPosition];
        pieceMoving.GlobalPosition = cellToMoveTo.GlobalPosition;
        
        Pieces.Remove(fromPosition);
        Pieces.Add(toPosition, pieceMoving);
    }

    public void MovePieceOrCapture(CellPosition fromPosition, CellPosition toPosition)
    {
        var cellToMoveTo = BoardManager.GetCellByPosition(toPosition);
        
        var pieceMoving = Pieces[fromPosition];
        
        Pieces.TryGetValue(toPosition, out var capturedPiece);

        if (capturedPiece != null)
        {
            capturedPiece.QueueFree();
            Pieces.Remove(toPosition);
        }
        
        Pieces.Remove(fromPosition);
        
        var tween = GetTree().CreateTween();
        tween.TweenProperty(pieceMoving, "position", cellToMoveTo.GlobalPosition, 0.2f);
        
        //pieceMoving.GlobalPosition = cellToMoveTo.GlobalPosition;
        Pieces.Add(toPosition, pieceMoving);
    }
}