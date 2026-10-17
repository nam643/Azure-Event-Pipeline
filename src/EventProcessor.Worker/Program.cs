using EventProcessor.Worker;
using EventProcessor.Worker.Services;
using EventProcessor.Worker.Configuration;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IEventSource, JsonFileEventSource>();
builder.Services.AddSingleton<BatchSummarizer>();
builder.Services
    .AddOptions<WorkerOptions>()
    .Bind(builder.Configuration.GetSection(
        WorkerOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var host = builder.Build();
host.Run();