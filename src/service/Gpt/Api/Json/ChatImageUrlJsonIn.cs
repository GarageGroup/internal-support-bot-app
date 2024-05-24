using System;
using System.Text.Json.Serialization;

namespace GarageGroup.Internal.Support.Json;

internal sealed record class ChatImageUrlJsonIn
{
    public ChatImageUrlJsonIn(string? url)
    {
        ImageUrl = url.OrEmpty();
    }

    [JsonPropertyName("url")]
    public string? ImageUrl { get; init; }
}