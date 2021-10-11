using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace GGroupp.Internal.Support.Bot;

internal sealed record CustomerChooseValueJson
{
    [JsonProperty("id")]
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonProperty("title")]
    [JsonPropertyName("title")]
    public string? Title { get; init; }
}

