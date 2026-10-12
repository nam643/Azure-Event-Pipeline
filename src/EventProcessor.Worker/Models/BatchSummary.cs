namespace EventProcessor.Worker.Models;

public record BatchSummary(
    Guid BatchId,
    DateTimeOffset ProcessedAt,
    int EventCount,
    IReadOnlyList<SymbolSummary> Symbols
);