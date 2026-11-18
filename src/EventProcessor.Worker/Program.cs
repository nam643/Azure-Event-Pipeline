using Azure.Storage.Blobs;
using EventProcessor.Worker;
using EventProcessor.Worker.Services;
using EventProcessor.Worker.Configuration;
using Microsoft.Extensions.Options;
using Azure.Identity;
using Azure.Core;
using System.Data.Common;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
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

        //Azure connection string
        var blobsConnectionString = builder.Configuration.GetConnectionString("blobs");

        if (!string.IsNullOrWhiteSpace(blobsConnectionString))
        {
            var csBuilder = new DbConnectionStringBuilder
            {
                ConnectionString = blobsConnectionString
            };

            if (!csBuilder.TryGetValue("ContainerName", out var containerNameValue))
            {
                throw new InvalidOperationException("ContainerName missing from Aspire blob configuration");
            }

            string containerName = containerNameValue.ToString()!;

            // csBuilder.Remove("ContainerName");

            // Console.WriteLine("=== RAW ===");
            // Console.WriteLine(blobsConnectionString);

            // Console.WriteLine("=== PARSED ===");
            // Console.WriteLine(csBuilder.ConnectionString);

            // Console.WriteLine("=== CONTAINER ===");
            // Console.WriteLine(containerName);

            // return new BlobContainerClient(csBuilder.ConnectionString, containerName);

            string storageConnectionString = blobsConnectionString.Replace(
                                            $";ContainerName={containerName}",
                                            string.Empty,
                                            StringComparison.OrdinalIgnoreCase);

            return new BlobContainerClient(
                storageConnectionString,
                containerName);
        }

        // real azure account
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