using System;
using Newtonsoft.Json;

namespace GGroupp.Internal.Support;

internal sealed record class IncidentLinkFlowState
{
    [JsonProperty("id")]
    public Guid Id { get; init; }

    [JsonProperty("title")]
    public string? Title { get; init; }
}