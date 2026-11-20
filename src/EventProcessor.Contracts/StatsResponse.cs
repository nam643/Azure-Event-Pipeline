namespace EventProcessor.Contracts;

public record StatsResponse(
    int BatchCount,
    int EventCount,
    IReadOnlyList<SymbolSummary> Symbols
);