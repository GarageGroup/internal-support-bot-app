using GarageGroup.Internal.Support.Json;
using System;
using System.Text.Json.Serialization;

namespace GarageGroup.Internal.Support;

internal sealed record class ChatMessageJsonIn
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public FlatArray<ChatContentJsonIn> Content { get; init; }
}