using System.Text.Json.Serialization;

namespace GarageGroup.Internal.Support;

internal sealed record class ChatMessageJsonOut
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }
}