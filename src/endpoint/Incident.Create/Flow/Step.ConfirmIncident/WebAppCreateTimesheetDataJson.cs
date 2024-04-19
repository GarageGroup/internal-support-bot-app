using GarageGroup.Internal.Support.FlowState;
using Newtonsoft.Json;

namespace GarageGroup.Internal.Support;

internal readonly record struct WebAppCreateSupportDataJson
{
    [JsonProperty("title")]
    public string? Title { get; init; }

    [JsonProperty("customer")]
    public IncidentCustomerState? Customer { get; init; }

    [JsonProperty("contact")]
    public IncidentContactState? Contact { get; init; }

    [JsonProperty("caseTypeCode")]
    public IncidentCaseTypeCode CaseTypeCode { get; init; }

    [JsonProperty("priorityCode")]
    public IncidentPriorityCode PriorityCode { get; init; }

    [JsonProperty("owner")]
    public IncidentOwnerState? Owner { get; init; }

    [JsonProperty("description")]
    public string? Description { get; init; }
}