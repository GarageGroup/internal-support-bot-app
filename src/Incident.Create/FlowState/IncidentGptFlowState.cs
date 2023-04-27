using Newtonsoft.Json;

namespace GGroupp.Internal.Support;

internal sealed record class IncidentGptFlowState
{
    [JsonProperty("title")]
    public string? Title { get; init; }

    [JsonProperty("temporaryActivityId")]
    public string? TemporaryActivityId { get; init; }
}