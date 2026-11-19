namespace EventProcessor.Contracts;

public record BatchSummary(
    Guid BatchId,
    DateTimeOffset ProcessedAt,
    int EventCount,
    IReadOnlyList<SymbolSummary> Symbols
);