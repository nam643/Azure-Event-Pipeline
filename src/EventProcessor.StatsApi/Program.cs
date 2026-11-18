using Azure.Storage.Blobs;

var builder = WebApplication.CreateBuilder(args);

builder.AddAzureBlobContainerClient("blobs");

builder.AddServiceDefaults();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/stats", async (
    BlobContainerClient container,
    CancellationToken cancellationToken) =>
{
    return "todo";
});

app.Run();
