using System;
using Newtonsoft.Json;

namespace GarageGroup.Internal.Support;

internal sealed record class IncidentCreateFlowState
{
    [JsonProperty("dbMinDate")]
    public DateTime DbMinDate { get; init; }

    [JsonProperty("botUserId")]
    public Guid? BotUserId { get; init; }

    [JsonProperty("botUserName")]
    public string? BotUserName { get; init; }

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

    [JsonProperty("temporaryActivityId")]
    public string? TemporaryActivityId { get; init; }

    [JsonProperty("gpt")]
    public IncidentGptFlowState Gpt { get; init; }
}