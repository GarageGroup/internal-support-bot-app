using Newtonsoft.Json;

namespace GGroupp.Internal.Support;

internal sealed record class IncidentLinkFlowState
{
    [JsonProperty("title")]
    public string? Title { get; init; }

    [JsonProperty("url")]
    public string? Url { get; init; }
}