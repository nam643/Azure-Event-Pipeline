using EventProcessor.Worker;
using EventProcessor.Worker.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IEventSource, JsonFileEventSource>();
builder.Services.AddSingleton<BatchSummarizer>();

var host = builder.Build();
host.Run();
