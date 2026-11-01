using CheckTheGreat.Domain.Matches;

namespace CheckTheGreat.AI.Search;

public sealed record SearchResult(Move? Move, float Score, int CompletedDepth, long VisitedNodes);