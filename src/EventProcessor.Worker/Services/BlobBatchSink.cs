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
            await _containerClient.CreateIfNotExistsAsync(
                cancellationToken : cancellationToken
            );

            _containerCreated = true;
        }

        string blobName = $"{summary.ProcessedAt.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture)}/" + $"{summary.BatchId}.json";

        BlobClient blobClient = _containerClient.GetBlobClient(blobName);

        BinaryData data = BinaryData.FromObjectAsJson(
            summary,
            JsonOptions
        );

        await blobClient.UploadAsync(
            data,
            overwrite: true,
            cancellationToken
        );

        _logger.LogInformation(
            "Wrote blob {blobName}",
            blobName
        );
    }
}