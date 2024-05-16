using System;
using System.Collections.Generic;
using GarageGroup.Internal.Support.FlowState;
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

    [JsonProperty("owner")]
    public IncidentOwnerState? Owner { get; init; }

    [JsonProperty("customer")]
    public IncidentCustomerState? Customer { get; init; }

    [JsonProperty("contact")]
    public IncidentContactState? Contact { get; init; }

    [JsonProperty("title")]
    public string? Title { get; init; }

    [JsonProperty("description")]
    public IncidentValueState? Description { get; init; }

    [JsonProperty("caseTypeCode")]
    public IncidentCaseTypeCode? CaseTypeCode { get; init; }

    [JsonProperty("caseTypeTitle")]
    public string? CaseTypeTitle { get; init; }

    [JsonProperty("priorityCode")]
    public IncidentPriorityCode PriorityCode { get; init; }

    [JsonProperty("priorityTitle")]
    public string? PriorityTitle { get; init; }

    [JsonProperty("temporaryActivityId")]
    public string? TemporaryActivityId { get; init; }

    [JsonProperty("incidentId")]
    public Guid IncidentId { get; init; }

    [JsonProperty("gpt")]
    public IncidentGptFlowState Gpt { get; init; }

    [JsonProperty("urlWebApp")]
    public string? UrlWebApp {  get; init; }

    [JsonProperty("withoutConfirmation")]
    public bool WithoutConfirmation { get; set; }

    [JsonProperty("telegramSender")]
    public TelegramSenderState? TelegramSender { get; init; }

    [JsonProperty("isNotFirstLaunch")]
    public bool IsNotFirstLaunch { get; set; }
}