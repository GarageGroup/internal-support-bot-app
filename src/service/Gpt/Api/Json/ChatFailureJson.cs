using System.Text.Json.Serialization;

namespace GarageGroup.Internal.Support;

internal sealed record class ChatFailureJson
{
    [JsonPropertyName("error")]
    public ChatErrorInfoJson? Error { get; init; }
}