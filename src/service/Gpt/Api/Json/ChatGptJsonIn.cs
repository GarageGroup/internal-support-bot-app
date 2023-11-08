using System;
using System.Text.Json.Serialization;

namespace GarageGroup.Internal.Support;

internal sealed record class ChatGptJsonIn
{
    [JsonPropertyName("messages")]
    public FlatArray<ChatMessageJson> Messages { get; init; }

    [JsonPropertyName("model")]
    public string? Model { get; init; }

    [JsonPropertyName("max_tokens")]
    public int? MaxTokens { get; init; }

    [JsonPropertyName("top_p")]
    public int? Top { get; init; }

    [JsonPropertyName("temperature")]
    public decimal? Temperature { get; init; }
}