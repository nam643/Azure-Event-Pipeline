namespace EventProcessor.Worker.Services;
using EventProcessor.Worker.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

public sealed class JsonFileEventSource : IEventSource // sealed: common when don't expect inheritance
{
    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    private IReadOnlyList<TradeEvent>? _events;
    private int _cursor;
    private readonly string _path;

    public JsonFileEventSource()
    {
        _path = Path.Combine(
            AppContext.BaseDirectory,
            "Data",
            "events.json"
        );
    }

    public async Task<IReadOnlyList<TradeEvent>> GetNextBatchAsync(int maxCount, CancellationToken cancellationToken)
    {
        _events ??= await LoadEventAsync(cancellationToken);

        List<TradeEvent> batch = [];

        for (int i = 0; i < maxCount; i++)
        {
            batch.Add(_events[_cursor]);
            _cursor = (_cursor + 1) % _events.Count;
        }

        return batch;
    }

    private async Task<IReadOnlyList<TradeEvent>> LoadEventAsync(CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(_path);

        var events = await JsonSerializer.DeserializeAsync<List<TradeEvent>>(stream, JsonOptions, cancellationToken);

        if (events is null || events.Count == 0)
        {
            throw new InvalidOperationException($"No events found in '{_path}'.");
        }

        return events;
    }

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        options.Converters.Add(new JsonStringEnumConverter());

        return options;
    }
}
