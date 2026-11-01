using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CheckTheGreat.Godot.scripts;

public class PieceInitialization
{
    [JsonPropertyName("piece_type")] public string PieceTypeJson { get; set; }
    [JsonIgnore] public PieceType PieceType => Enum.Parse<PieceType>(PieceTypeJson);
    [JsonPropertyName("starting_position")] public string StartingPositionJson { get; set; }
    
    [JsonIgnore]
    public CellPosition StartingPosition => CellPosition.FromJson(StartingPositionJson);
    

    public string ToJson()
    {
        return $"{StartingPosition.Col},{StartingPosition.Row}";
    }

    public static PieceInitialization FromJson(string json)
    {
        var enemyInitialization = JsonSerializer.Deserialize<PieceInitialization>(json);
        return enemyInitialization;
    }
}