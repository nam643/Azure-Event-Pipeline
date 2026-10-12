using EventProcessor.Worker.Services;

namespace EventProcessor.Worker;

public class Worker(ILogger<Worker> logger, IEventSource eventSource) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
    private readonly IEventSource _eventSource = eventSource;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var events = await _eventSource.GetNextBatchAsync(3, stoppingToken);
            if (_logger.IsEnabled(LogLevel.Information))
            {
                foreach (var trade in events)
                {
                    _logger.LogInformation("Symbol={Symbol} Side={Side} Qty={Qty}", trade.Symbol, trade.Side, trade.Qty);
                }
            }
            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }
}
