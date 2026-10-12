namespace EventProcessor.Worker.Services;
using EventProcessor.Worker.Models; //this makes me see the TradeEvent.cs

public interface IEventSource
{
    Task<IReadOnlyList<TradeEvent>> GetNextBatchAsync(int maxCount, CancellationToken cancellationToken);
    // maxCount: How many events to return
}