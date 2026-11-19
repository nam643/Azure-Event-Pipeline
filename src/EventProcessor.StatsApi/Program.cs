using System.Text.Json;
using Azure.Storage.Blobs;
using EventProcessor.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.AddAzureBlobContainerClient("blobs");

builder.AddServiceDefaults();

var app = builder.Build();

app.MapDefaultEndpoints();

// app.MapGet("/stats", async (
//     BlobContainerClient container,
//     CancellationToken cancellationToken) =>
// {
//     var summaries = new List<BatchSummary>();

//     await foreach(var blob in container.GetBlobsAsync(cancellationToken : cancellationToken))
//     {
//         var blobClient = container.GetBlobClient(blob.Name);

//         var response = await blobClient.DownloadContentAsync(cancellationToken);

//         var summary = JsonSerializer.Deserialize<BatchSummary>(response.Value.Content.ToString());

//         if(summary is not null)
//         {
//             summaries.Add(summary);
//         }
//     }

//     return Results.Ok(summaries);
// });

// app.MapGet("/stats", async (
//     BlobContainerClient container,
//     CancellationToken cancellationToken) =>
// {
//     var results = new List<object>();

//     await foreach (var blob in container.GetBlobsAsync(
//         cancellationToken: cancellationToken))
//     {
//         var blobClient = container.GetBlobClient(blob.Name);

//         var response =
//             await blobClient.DownloadContentAsync(
//                 cancellationToken);

//         results.Add(new
//         {
//             BlobName = blob.Name,
//             Json = response.Value.Content.ToString()
//         });
//     }

//     return Results.Ok(results);
// });

app.MapGet("/stats", async (
    BlobContainerClient container,
    CancellationToken cancellationToken) =>
{
    var summaries = new List<BatchSummary>();

    await foreach (var blob in container.GetBlobsAsync(
        cancellationToken: cancellationToken))
    {
        var blobClient =
            container.GetBlobClient(blob.Name);

        var response =
            await blobClient.DownloadContentAsync(
                cancellationToken);

        var summary = response.Value.Content.ToObjectFromJson<BatchSummary>(JsonSettings.Default);

        if (summary is not null)
        {
            summaries.Add(summary);
        }
    }

    return Results.Ok(summaries);
});

app.Run();
