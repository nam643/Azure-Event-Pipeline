using System.Text.Json;

namespace EventProcessor.Contracts;

public static class JsonSettings
{
    public static readonly JsonSerializerOptions Default =
        new(JsonSerializerDefaults.Web);
}