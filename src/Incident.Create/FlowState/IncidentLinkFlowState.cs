using System;
using Newtonsoft.Json;

namespace GarageGroup.Internal.Support;

internal sealed record class IncidentLinkFlowState
{
    [JsonProperty("id")]
    public Guid Id { get; init; }

    [JsonProperty("title")]
    public string? Title { get; init; }

    [JsonProperty("temporaryActivityId")]
    public string? TemporaryActivityId { get; init; }

    [JsonProperty("gpt")]
    public IncidentGptFlowState Gpt { get; init; }
}