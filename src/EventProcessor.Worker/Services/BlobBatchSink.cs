using Azure.Storage.Blobs.Models;
using System.Globalization;
using System.Text.Json;
using Azure.Storage.Blobs;
using EventProcessor.Worker.Models;

namespace EventProcessor.Worker.Services;

public sealed class BlobBatchSink(
    BlobContainerClient containerClient,
    ILogger<BlobBatchSink> logger) : IBatchSink
{
    private readonly BlobContainerClient _containerClient = containerClient;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private bool _containerCreated;
    private readonly ILogger<BlobBatchSink> _logger = logger;

    public async Task WriteAsync(
        BatchSummary summary,
        CancellationToken cancellationToken)
    {
        if (!_containerCreated)
        {
            _logger.LogInformation("Creating container...");
            await _containerClient.CreateIfNotExistsAsync(
                cancellationToken : cancellationToken
            );

            _containerCreated = true;
        }


        string blobName = $"{summary.ProcessedAt:yyyy/MM/dd}/{summary.BatchId}.json";

        BlobClient blobClient = _containerClient.GetBlobClient(blobName);


        BinaryData data = BinaryData.FromObjectAsJson(
            summary,
            JsonOptions
        );

        var options = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = "application/json"
            }
        };

        await blobClient.UploadAsync(
            data,
            options,
            cancellationToken
        );

        _logger.LogInformation(
            "Wrote blob {BlobName}",
            blobName
        );

    }
}