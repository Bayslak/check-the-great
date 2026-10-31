using System;
using System.IO;
using ChecktheGreat.scripts;
using Godot;

public partial class GameManager : Node
{
    [Export] private BoardManager _boardManager;
    [Export] private PieceManager _pieceManager;

    public Action<GameState> GameStateChanged;
    public GameState CurrentGameState;
    
    [Export] private PlayerPiecePlacer _playerPiecePlacer;

    private Level _currentLevel;
    
    public override void _Ready()
    {
        CurrentGameState = GameState.Initialization;
        EnterState(CurrentGameState);
    }

    private void ChangeGameState(GameState newState)
    {
        if (newState == CurrentGameState) return;
        
        ExitState(CurrentGameState);
        CurrentGameState = newState;
        EnterState(newState);
        
        GameStateChanged?.Invoke(newState);
    }
    
    private void EnterState(GameState enteredState)
    {
        switch (enteredState)
        {
            case GameState.Initialization:
                StartLevel();
                ChangeGameState(GameState.PiecesPlacement);
                break;
            case GameState.PiecesPlacement:
                _pieceManager.InitializePieces(_currentLevel);
                _playerPiecePlacer.Enable(2, _currentLevel.StartingTiles);
                _boardManager.HighlightStartingTiles(_currentLevel.StartingTiles);
                break;
            case GameState.Level:
                //TODO: implement turn system
                break;
        }
        
        GD.Print("Entered state: " + enteredState);
    }

    private void ExitState(GameState exitedState)
    {
        switch (exitedState)
        {
            case GameState.PiecesPlacement:
                _boardManager.DeHighlightTiles();
                break;
            case GameState.Level:
                //TODO: clean up level
                break;
        }
    }
    
    private void StartLevel()
    {
        string levelJson = File.ReadAllText("resources/level_test.json");
        var level = Level.LoadFromJson(levelJson);
        _currentLevel = level;
        
        _boardManager.InitializeBoard(level);
        //_pieceManager.InitializePieces(level);
    }

    public void PlayerPiecePlacerDisabled()
    {
        ChangeGameState(GameState.Level);
    }
}