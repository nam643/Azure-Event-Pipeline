using EventProcessor.Worker.Models;

public interface IBatchSink
{
    Task WriteAsync(
        BatchSummary summary, CancellationToken cancellationToken        
    );
}