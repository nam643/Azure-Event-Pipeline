using EventProcessor.Contracts;


namespace EventProcessor.Worker.Services;

public interface IBatchSink
{
    Task WriteAsync(
        BatchSummary summary, CancellationToken cancellationToken
    );
}