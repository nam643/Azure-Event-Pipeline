using EventProcessor.Worker.Models;

namespace EventProcessor.Worker.Services;

public class LoggingBatchSink(ILogger<LoggingBatchSink> logger) : IBatchSink
{
    private readonly ILogger<LoggingBatchSink> _logger = logger;

    public Task WriteAsync(
        BatchSummary summary,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInformation(
            "Writing batch {BatchId} with {EventCount} events and {SymbolCount} symbols",
            summary.BatchId,
            summary.EventCount,
            summary.Symbols.Count
        );

        return Task.CompletedTask;
    }
}