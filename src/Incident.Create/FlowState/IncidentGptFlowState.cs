using Newtonsoft.Json;

namespace GGroupp.Internal.Support;

internal readonly record struct IncidentGptFlowState
{
    [JsonProperty("title")]
    public string? Title { get; init; }

    [JsonProperty("sourceMessage")]
    public string? SourceMessage { get; init; }

    [JsonProperty("temporaryActivityId")]
    public string? TemporaryActivityId { get; init; }

    [JsonProperty("errorMessage")]
    public string? ErrorMessage { get; init; }
}