using System.Text.Json.Serialization;

namespace GarageGroup.Internal.Support.Service.Gpt.Test;

internal sealed record class StubGptJsonOut
{
    [JsonPropertyName("choices")]
    public StubChoiceJson[]? Choices { get; init; }
}