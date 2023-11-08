using System.Text.Json.Serialization;

namespace GarageGroup.Internal.Support;

internal sealed record class ChatChoiceJson
{
    [JsonPropertyName("message")]
    public ChatMessageJson? Message { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}