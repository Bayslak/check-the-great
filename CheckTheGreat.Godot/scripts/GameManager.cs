using System;
using CheckTheGreat.AI.Evaluation;
using CheckTheGreat.AI.Search;
using CheckTheGreat.Application.Matches;
using CheckTheGreat.Domain.Matches;
using CheckTheGreat.Domain.Rules;
using Godot;

namespace CheckTheGreat.Godot.scripts;

public partial class GameManager : Node
{
    [Export] private BoardManager _boardManager;
    [Export] private PieceManager _pieceManager;
    [Export] private TurnManager _turnManager;

    public Action<GameState> GameStateChanged;
    public GameState CurrentGameState;
    
    [Export] private PlayerPiecePlacer _playerPiecePlacer;

    private Level _currentLevel;

    public MatchController Match;
    public MatchSettings MatchSettings;
    
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
                //_playerPiecePlacer.Enable(2, _currentLevel.StartingTiles);
                //_boardManager.HighlightStartingTiles(_currentLevel.StartingTiles);
                break;
            case GameState.Level:
                _turnManager.Enable(this, _boardManager, _pieceManager);
                break;
        }
        
        GD.Print("Entered state: " + enteredState);
    }

    private void HandlePiecesInitialized()
    {
        ChangeGameState(GameState.Level);
    }

    private void ExitState(GameState exitedState)
    {
        switch (exitedState)
        {
            case GameState.PiecesPlacement:
                //_boardManager.DeHighlightTiles();
                //_playerPiecePlacer.Disable();
                break;
            case GameState.Level:
                _turnManager.Disable();
                break;
        }
    }
    
    private void StartLevel()
    {
        var gameState = InitialPositionFactory.CreateStandard();
        var gameLevel = Level.LoadFromGameState(gameState);
        
        MatchSettings = new MatchSettings(
            WhiteController: ControllerType.Human,
            BlackController: ControllerType.Cpu);
        
        var rules = new GameRules();
        
        Match = new MatchController(
            gameState,
            rules,
            MatchSettings,
            whiteAi: new MinimaxAi(rules, new Evaluator()),
            blackAi: new MinimaxAi(rules, new Evaluator()));
        
        //string levelJson = File.ReadAllText("resources/level_test.json");
        //var level = Level.LoadFromJson(levelJson);
        _currentLevel = gameLevel;
        
        _boardManager.InitializeBoard(_currentLevel);
        //_pieceManager.InitializePieces(level);
        
        _pieceManager.PiecesInitialized += HandlePiecesInitialized;
    }

    public void PlayerPiecePlacerDisabled()
    {
        ChangeGameState(GameState.Level);
    }

    public void UseMoveResult(MoveResult moveResult)
    {
        var fromPosition = new CellPosition(moveResult.Move.From.X + 1, moveResult.Move.From.Y + 1);
        var toPosition =  new CellPosition(moveResult.Move.To.X + 1, moveResult.Move.To.Y + 1);

        switch (moveResult.Move.Mode)
        {
            case MoveMode.MoveOnly:
                _pieceManager.MovePiece(fromPosition, toPosition);
                break;
            case MoveMode.CaptureOnly:
            case MoveMode.MoveOrCapture:
                _pieceManager.MovePieceOrCapture(fromPosition, toPosition);
                break;
            default:
                GD.PrintErr($"Unknown move mode: {moveResult.Move.Mode}. Rotty didn't implemented it.");
                break;
        }

        _turnManager.AdvanceTurn();
    }
}