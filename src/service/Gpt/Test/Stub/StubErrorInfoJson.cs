using System.Text.Json.Serialization;

namespace GarageGroup.Internal.Support.Service.Gpt.Test;

internal sealed record class StubErrorInfoJson
{
    [JsonPropertyName("message")]
    public string? Message { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }
}