using Azure.Storage.Blobs;
using EventProcessor.Worker;
using EventProcessor.Worker.Services;
using EventProcessor.Worker.Configuration;
using Microsoft.Extensions.Options;

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

builder.Services.AddSingleton<IBatchSink, BlobBatchSink>();

builder.Services
    .AddOptions<BlobStorageOptions>()
    .Bind(builder.Configuration.GetSection
        (BlobStorageOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<BlobContainerClient>(
    sp =>
    {
        var options = sp.GetRequiredService<IOptions<BlobStorageOptions>>();

        return new BlobContainerClient(options.Value.ConnectionString,options.Value.ContainerName);
    }
);

var host = builder.Build();
host.Run();