using Microsoft.Extensions.Options;
using EventProcessor.Worker.Configuration;
using EventProcessor.Worker.Services;


namespace EventProcessor.Worker;

public class Worker(ILogger<Worker> logger,
    IEventSource eventSource,
    BatchSummarizer summarizer,
    IOptions<WorkerOptions> options) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
    private readonly IEventSource _eventSource = eventSource;
    private readonly BatchSummarizer _summarizer = summarizer;
    private readonly WorkerOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var events = await _eventSource.GetNextBatchAsync(_options.BatchSize, stoppingToken);

                    var summary = _summarizer.Summarize(events, DateTimeOffset.UtcNow);
                    _logger.LogInformation("BatchId={BatchId} EventCount={EventCount} SymbolCount={SymbolCount}", summary.BatchId, summary.EventCount, summary.Symbols.Count);

                    foreach (var symbol in summary.Symbols)
                    {
                        _logger.LogInformation(
                            "Symbol={Symbol} Trades={TradeCount} BuyQty={BuyQty} SellQty={SellQty} VWAP={Vwap}",
                            symbol.Symbol,
                            symbol.TradeCount,
                            symbol.BuyQty,
                            symbol.SellQty,
                            symbol.Vwap
                        );
                    }
                }
                catch(Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Error while processing batch");
                }

                await Task.Delay(TimeSpan.FromSeconds(_options.IntervalSeconds), stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Worker stopping");
        }
    }
}
