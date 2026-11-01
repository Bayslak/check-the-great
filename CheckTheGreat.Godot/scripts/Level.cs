using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using CheckTheGreat.Domain.Matches;

namespace CheckTheGreat.Godot.scripts;

public class Level
{
    [JsonPropertyName("name")] public string LevelName { get; set; }
    [JsonPropertyName("board_rows")] public int Rows { get; set; }
    [JsonPropertyName("board_columns")] public int Columns { get; set; }
    [JsonPropertyName("empty_tiles")] public string[] EmptyTilesJson { get; set; } = [];
    [JsonPropertyName("starting_tiles")] public string[] StartingTilesJson { get; set; } = [];
    [JsonPropertyName("enemies")] public PieceInitialization[] Enemies { get; set; } = [];
    [JsonPropertyName("friendlies")] public PieceInitialization[] Friendlies { get; set; } = [];
    
    [JsonIgnore]
    public HashSet<CellPosition> EmptyTiles;
    
    [JsonIgnore]
    public HashSet<CellPosition> StartingTiles;

    public string ToJson()
    {
        EmptyTilesJson = EmptyTiles.Select(et => et.ToJson()).ToArray();
        StartingTilesJson = StartingTiles.Select(et => et.ToJson()).ToArray();
        return JsonSerializer.Serialize(this);
    }

    public static Level LoadFromJson(string json)
    {
        var level = JsonSerializer.Deserialize<Level>(json);
        level.SetEmptyTilesFromJson();
        level.SetStartingTilesFromJson();
        return level;
    }

    private void SetEmptyTilesFromJson()
    {
        EmptyTiles = EmptyTilesJson.Select(CellPosition.FromJson).ToHashSet();
    }

    private void SetStartingTilesFromJson()
    {
        StartingTiles = StartingTilesJson.Select(CellPosition.FromJson).ToHashSet();
    }

    public static Level LoadFromGameState(Domain.Matches.GameState gameState)
    {
        var enemies = new List<PieceInitialization>();
        var friendlies = new List<PieceInitialization>();
        
        foreach (var kvp in gameState.Pieces)
        {
            var startingPositionJson = $"{kvp.Key.X + 1},{kvp.Key.Y + 1}";
            var pieceInitialization = new PieceInitialization()
            {
                Id = kvp.Value.Id.ToString(),
                PieceTypeJson = kvp.Value.GetPieceType().ToString(),
                StartingPositionJson = startingPositionJson
            };

            if (kvp.Value.Owner == Side.Black)
                enemies.Add(pieceInitialization);
            else
                friendlies.Add(pieceInitialization);
        }
        
        var level = new Level()
        {
            LevelName = "Hello",
            Columns = gameState.Board.Width,
            Rows = gameState.Board.Height,
            Enemies = enemies.ToArray(),
            Friendlies = friendlies.ToArray(),
            StartingTilesJson = [],
            EmptyTilesJson = []
        };
        
        level.SetEmptyTilesFromJson();
        level.SetStartingTilesFromJson();

        return level;
    }
}