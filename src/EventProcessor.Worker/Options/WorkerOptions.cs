using System.ComponentModel.DataAnnotations;

namespace EventProcessor.Worker.Options;

public class WorkerOptions
{
    public const string SectionName = "EventProcessor";

    [Range(1, 1000)]
    public int BatchSize { get; set; } = 3;

    [Range(1, 3600)]
    public int IntervalSeconds { get; set; } = 2;
}