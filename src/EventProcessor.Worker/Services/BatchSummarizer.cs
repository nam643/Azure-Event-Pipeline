using EventProcessor.Worker.Models;

namespace EventProcessor.Worker.Services;

public class BatchSummarizer
{
    public static BatchSummary Summarize(IReadOnlyList<TradeEvent> events, DateTimeOffset processedAt)
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

                    return new SymbolSummary(
                        group.Key,
                        group.Count(),
                        buyQty,
                        sellQty,
                        totalValue/totalQty
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