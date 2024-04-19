using Newtonsoft.Json;
using System;

namespace GarageGroup.Internal.Support.FlowState;

internal sealed record class IncidentOwnerState
{
    [JsonProperty("id")]
    public Guid Id { get; init; }

    [JsonProperty("fullName")]
    public string? FullName { get; init; }
}

