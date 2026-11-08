using Azure.Storage.Blobs;
using EventProcessor.Worker;
using EventProcessor.Worker.Services;
using EventProcessor.Worker.Configuration;
using Microsoft.Extensions.Options;
using Azure.Identity;
using Azure.Core;

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

        if (!string.IsNullOrWhiteSpace(options.Value.AccountUri))
        {
            var containerUri = new Uri(
                new Uri(options.Value.AccountUri),
                options.Value.ContainerName
            );

            TokenCredential credential = builder.Environment.IsDevelopment()
                ? new AzureCliCredential()
                : new ManagedIdentityCredential();

            return new BlobContainerClient(containerUri, credential);
        }

        return new BlobContainerClient(options.Value.ConnectionString, options.Value.ContainerName);
    }
);

var host = builder.Build();
host.Run();