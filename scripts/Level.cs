using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChecktheGreat.scripts;

public class Level
{
    [JsonPropertyName("name")] public string LevelName { get; set; }
    [JsonPropertyName("board_rows")] public int Rows { get; set; }
    [JsonPropertyName("board_columns")] public int Columns { get; set; }
    [JsonPropertyName("empty_tiles")] public string[] EmptyTilesJson { get; set; } = [];
    
    [JsonIgnore]
    public HashSet<CellPosition> EmptyTiles;

    public string ToJson()
    {
        EmptyTilesJson = EmptyTiles.Select(et => et.ToJson()).ToArray();
        return JsonSerializer.Serialize(this);
    }

    public static Level LoadFromJson(string json)
    {
        var level = JsonSerializer.Deserialize<Level>(json);
        level.SetEmptyTilesFromJson();
        return level;
    }

    private void SetEmptyTilesFromJson()
    {
        EmptyTiles = EmptyTilesJson.Select(CellPosition.FromJson).ToHashSet();
    }
}