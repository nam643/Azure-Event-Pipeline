using System.Dynamic;

namespace EventProcessor.Worker.Options;

public class WorkerOptions
{
    public const string SectionName = "EventProcessor";

    public int BatchSize {get;set;} = 3; 

    public int IntervalSeconds {get;set;} = 2;
}