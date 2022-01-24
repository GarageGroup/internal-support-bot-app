using System;
using Newtonsoft.Json;

namespace GGroupp.Internal.Support;

internal sealed record IncidentCreateFlowState
{
    [JsonProperty("ownerId")]
    public Guid OwnerId { get; init; }

    [JsonProperty("customerId")]
    public Guid CustomerId { get; init; }

    [JsonProperty("customerTitle")]
    public string? CustomerTitle { get; init; }

    [JsonProperty("contactId")]
    public Guid ContactId { get; init; }

    [JsonProperty("contactFullName")]
    public string? ContactFullName{ get; init; }

    [JsonProperty("title")]
    public string? Title { get; init; }

    [JsonProperty("description")]
    public string? Description { get; init; }

    [JsonProperty("caseTypeCode")]
    public int CaseTypeCode { get; init; }

    [JsonProperty("caseTypeTitle")]
    public string? CaseTypeTitle{  get; init; }
}