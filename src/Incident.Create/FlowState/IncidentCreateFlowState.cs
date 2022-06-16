using System;
using Newtonsoft.Json;

namespace GGroupp.Internal.Support;

internal sealed record class IncidentCreateFlowState
{
    [JsonProperty("ownerId")]
    public Guid OwnerId { get; init; }

    [JsonProperty("ownerFullName")]
    public string? OwnerFullName { get; init; }

    [JsonProperty("customerId")]
    public Guid CustomerId { get; init; }

    [JsonProperty("customerTitle")]
    public string? CustomerTitle { get; init; }

    [JsonProperty("contactId")]
    public Guid? ContactId { get; init; }

    [JsonProperty("contactFullName")]
    public string? ContactFullName { get; init; }

    [JsonProperty("title")]
    public string? Title { get; init; }

    [JsonProperty("description")]
    public string? Description { get; init; }

    [JsonProperty("caseTypeCode")]
    public IncidentCaseTypeCode CaseTypeCode { get; init; }

    [JsonProperty("caseTypeTitle")]
    public string? CaseTypeTitle { get; init; }

    [JsonProperty("priorityCode")]
    public IncidentPriorityCode PriorityCode { get; init; }

    [JsonProperty("priorityTitle")]
    public string? PriorityTitle { get; init; }
}