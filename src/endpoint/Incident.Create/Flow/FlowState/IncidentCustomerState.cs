using Newtonsoft.Json;
using System;

namespace GarageGroup.Internal.Support.FlowState;

internal sealed record class IncidentCustomerState
{
    [JsonProperty("id")]
    public Guid Id { get; init; }

    [JsonProperty("title")]
    public string? Title { get; init; }
}

