using EventProcessor.Worker;
using EventProcessor.Worker.Services;
using EventProcessor.Worker.Options;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IEventSource, JsonFileEventSource>();
builder.Services.AddSingleton<BatchSummarizer>();
builder.Services.Configure<WorkerOptions>(builder.Configuration.GetSection(WorkerOptions.SectionName));

var host = builder.Build();
host.Run();
