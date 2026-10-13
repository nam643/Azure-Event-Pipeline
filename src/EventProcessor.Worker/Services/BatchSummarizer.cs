using EventProcessor.Worker.Models;

namespace EventProcessor.Worker.Services;

public class BatchSummarizer
{
    public BatchSummary Summarize(IReadOnlyList<TradeEvent> events, DateTimeOffset processedAt)
    {
        var symbols =
            events
                .GroupBy(e => e.Symbol)
                .Select(group =>
                {
                    var buyQty = group.Where(e => e.Side == Side.Buy).Sum(e => (long)e.Qty);

                    var sellQty = group.Where(e => e.Side == Side.Sell).Sum(e => (long)e.Qty);

                    var totalQty = group.Sum(e => (long)e.Qty);

                    var totalValue = group.Sum(e => e.Price * e.Qty);

                    if (totalQty == 0)
                    {
                        throw new InvalidOperationException($"Cannot calculate VWAP for symbol '{group.Key}' because total quantity is zero");
                    }

                    var vwap = Math.Round(totalValue / totalQty, 4);

                    return new SymbolSummary(
                        Symbol: group.Key,
                        TradeCount: group.Count(),
                        BuyQty: buyQty,
                        SellQty: sellQty,
                        Vwap: vwap
                    );
                })
                .OrderBy(s => s.Symbol)
                .ToList();

        return new BatchSummary(
            Guid.NewGuid(),
            processedAt,
            events.Count,
            symbols
        );
    }
}