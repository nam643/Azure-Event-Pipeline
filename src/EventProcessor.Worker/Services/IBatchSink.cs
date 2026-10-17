using EventProcessor.Worker.Models;

namespace EventProcessor.Worker.Services;

public interface IBatchSink
{
    Task WriteAsync(
        BatchSummary summary, CancellationToken cancellationToken        
    );
}