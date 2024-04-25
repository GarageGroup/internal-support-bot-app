using System;
using System.Text.Json.Serialization;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

internal sealed record class IncidentJsonCreateIn
{
    internal static DataverseEntityCreateIn<IncidentJsonCreateIn> BuildDataverseCreateInput(
        IncidentJsonCreateIn incidentJson)
        =>
        new(
            entityPluralName: "incidents",
            selectFields: IncidentJsonCreateOut.SelectedFields,
            entityData: incidentJson);

    internal static string BuildOwnerLookupValue(Guid ownerId)
        =>
        $"/systemusers({ownerId:D})";

    internal static string BuildCustomerLookupValue(Guid customerId)
        =>
        $"/accounts({customerId:D})";

    internal static string BuildContactLookupValue(Guid contactId)
        =>
        $"/contacts({contactId:D})";

    [JsonPropertyName("ownerid@odata.bind")]
    public string? OwnerId { get; init; }

    [JsonPropertyName("customerid_account@odata.bind")]
    public string? CustomerId { get; init; }

    [JsonPropertyName("primarycontactid@odata.bind")]
    public string? ContactId { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("casetypecode")]
    public int? CaseTypeCode { get; init; }

    [JsonPropertyName("prioritycode")]
    public int? PriorityCode { get; init; }

    [JsonPropertyName("caseorigincode")]
    public int? CaseOriginCode { get; init; }

    [JsonPropertyName("gg_sender_telegram_td"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SenderTelegramId { get; init; }
}