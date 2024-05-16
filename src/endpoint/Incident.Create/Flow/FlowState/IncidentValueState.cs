using Newtonsoft.Json;

namespace GarageGroup.Internal.Support.FlowState;

internal sealed record class IncidentValueState
{
    public IncidentValueState(string value)
        =>
        Value = value;

    [JsonProperty("value")]
    public string? Value { get; init; }
}
