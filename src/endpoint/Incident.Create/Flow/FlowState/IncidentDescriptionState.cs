using Newtonsoft.Json;

namespace GarageGroup.Internal.Support.FlowState;

internal sealed record class IncidentDescriptionState
{
    public IncidentDescriptionState(string value)
        =>
        Value = value;

    [JsonProperty("value")]
    public string? Value { get; init; }
}
