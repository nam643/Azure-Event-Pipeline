using EventProcessor.Worker;
using EventProcessor.Worker.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IEventSource, JsonFileEventSource>();

var host = builder.Build();
host.Run();
