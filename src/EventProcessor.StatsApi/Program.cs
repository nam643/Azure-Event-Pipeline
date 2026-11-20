using Azure.Storage.Blobs;
using EventProcessor.Contracts;
using Azure.Storage.Blobs.Models;

var builder = WebApplication.CreateBuilder(args);

builder.AddAzureBlobContainerClient("blobs");

builder.AddServiceDefaults();

builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("stats", policy =>
    {
        policy.Expire(TimeSpan.FromSeconds(10));
    });
});

var app = builder.Build();

app.UseOutputCache();

app.MapDefaultEndpoints();

app.MapGet("/stats", async (
    BlobContainerClient container,
    CancellationToken cancellationToken) =>
{
    var summaries = new List<BatchSummary>();

    var todayPrefix = DateTime.UtcNow.ToString("yyyy/MM/dd");

    await foreach (var blob in container.GetBlobsAsync(
        traits: BlobTraits.None,
        states: BlobStates.None,
        prefix: todayPrefix,
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

    var batchCount = summaries.Count;
    var eventCount = summaries.Sum(s => s.EventCount);
    var allSymbols = summaries.SelectMany(s => s.Symbols);
    var groupedSymbols = allSymbols.GroupBy(s => s.Symbol);

    var aggregatedSymbols =
            groupedSymbols.Select(g =>
            {
                var totalQuantity = g.Sum(s => s.BuyQty + s.SellQty);
                var totalValue = g.Sum(s => s.Vwap * (s.BuyQty + s.SellQty));

                var vwap = totalQuantity == 0 ? 0 : totalValue / totalQuantity;

                return new SymbolSummary(
                    g.Key,
                    g.Sum(s => s.TradeCount),
                    g.Sum(s => s.BuyQty),
                    g.Sum(s => s.SellQty),
                    Math.Round(vwap, 4)
                );
            });

    return Results.Ok(
        new StatsResponse(batchCount, eventCount, [.. aggregatedSymbols])
    );
})
.CacheOutput("stats");

app.Run();
