using System.Text.Json.Serialization;

namespace GarageGroup.Internal.Support.Service.Gpt.Test;

internal sealed record class StubChoiceJson
{
    [JsonPropertyName("message")]
    public StubMessageJson? Message { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}