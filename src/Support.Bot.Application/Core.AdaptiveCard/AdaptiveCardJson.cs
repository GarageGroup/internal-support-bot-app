using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace GGroupp.Infra;

public sealed record AdaptiveCardJson
{
    private const string DefaultType = "AdaptiveCard";

    private readonly string? type = DefaultType;

    private readonly object[]? body, actions;

    public AdaptiveCardJson(string version) => Version = version ?? string.Empty;

    [JsonProperty("version")]
    [JsonPropertyName("version")]
    public string Version { get; }

    [JsonProperty("type")]
    [JsonPropertyName("type")]
    public string Type
    {
        get => type ?? string.Empty;
        init => type = value;
    }

    [JsonProperty("body")]
    [JsonPropertyName("body")]
    public object[] Body
    {
        get => body ?? Array.Empty<object>();
        init => body = value;
    }

    [JsonProperty("actions")]
    [JsonPropertyName("actions")]
    public object[] Actions
    {
        get => actions ?? Array.Empty<object>();
        init => actions = value;
    }
}
