namespace EventProcessor.Contracts;

public record SymbolSummary(
    string Symbol,
    int TradeCount,
    long BuyQty,
    long SellQty,
    decimal Vwap
);