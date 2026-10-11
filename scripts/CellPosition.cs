namespace ChecktheGreat.scripts;

public readonly record struct CellPosition(int Col, int Row)
{
    public string ToJson()
    {
        return $"{Col},{Row}";
    }

    public static CellPosition FromJson(string json)
    {
        string[] positions = json.Split(',');
        return new CellPosition(int.Parse(positions[0]), int.Parse(positions[1]));
    }
}