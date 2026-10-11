using System;
using System.Collections.Generic;
using Godot;

namespace ChecktheGreat.scripts;

public partial class PieceManager : Node
{
    [Export] private PackedScene _piecePrefab;
    [Export] private Texture2D _blackPawn, _blackKnight, _blackBishop, _blackRook, _blackQueen, _blackKing;
    [Export] private Texture2D _whitePawn, _whiteKnight, _whiteBishop, _whiteRook, _whiteQueen, _whiteKing;
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

    public void SpawnPawn(CellPosition startingPosition, PieceType pieceType, PieceHouse pieceHouse)
    {
        var pawnSpawned = _piecePrefab.Instantiate<Piece>();
        AddChild(pawnSpawned);

        var texture2d = GuessTexture(pieceType, pieceHouse);
        pawnSpawned.SetSprite(texture2d);
        pawnSpawned.SetHouse(pieceHouse);
        
        var cell = BoardManager.GetCellByPosition(startingPosition);
        
        pawnSpawned.Position = cell.GlobalPosition;
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
}