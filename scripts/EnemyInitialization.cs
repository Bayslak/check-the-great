using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChecktheGreat.scripts;

public class EnemyInitialization
{
    [JsonPropertyName("starting_position")] public string StartingPositionJson { get; set; }
    
    [JsonIgnore]
    public CellPosition StartingPosition => CellPosition.FromJson(StartingPositionJson);

    public string ToJson()
    {
        return $"{StartingPosition.Col},{StartingPosition.Row}";
    }

    public static EnemyInitialization FromJson(string json)
    {
        var enemyInitialization = JsonSerializer.Deserialize<EnemyInitialization>(json);
        return enemyInitialization;
    }
}