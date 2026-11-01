namespace CheckTheGreat.AI.Search;

public sealed record SearchOptions(int MaxDepth = 3, long MaxNodes = 100_000);