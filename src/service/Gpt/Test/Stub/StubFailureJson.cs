using System.Text.Json.Serialization;

namespace GarageGroup.Internal.Support.Service.Gpt.Test;

internal sealed record class StubFailureJson
{
    [JsonPropertyName("error")]
    public StubErrorInfoJson? Error { get; init; }
}