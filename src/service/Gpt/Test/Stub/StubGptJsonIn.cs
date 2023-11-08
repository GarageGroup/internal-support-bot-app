using System.Text.Json.Serialization;

namespace GarageGroup.Internal.Support.Service.Gpt.Test;

internal sealed record class StubGptJsonIn
{
    [JsonPropertyName("messages")]
    public StubMessageJson[]? Messages { get; init; }

    [JsonPropertyName("model")]
    public string? Model { get; init; }

    [JsonPropertyName("max_tokens")]
    public int? MaxTokens { get; init; }

    [JsonPropertyName("top_p")]
    public int? Top { get; init; }

    [JsonPropertyName("temperature")]
    public decimal? Temperature { get; init; }
}