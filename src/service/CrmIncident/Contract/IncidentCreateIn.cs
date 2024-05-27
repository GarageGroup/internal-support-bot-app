using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Internal.Support;

public sealed record class IncidentCreateIn
{
    public IncidentCreateIn(
        Guid ownerId,
        Guid customerId,
        Guid? contactId,
        string title,
        [AllowNull] string description,
        IncidentCaseTypeCode caseTypeCode,
        IncidentPriorityCode priorityCode,
        Guid callerUserId)
    {
        OwnerId = ownerId;
        CustomerId = customerId;
        ContactId = contactId;
        Title = title.OrEmpty();
        Description = description.OrEmpty();
        CaseTypeCode = caseTypeCode;
        PriorityCode = priorityCode;
        CallerUserId = callerUserId;
    }

    public Guid OwnerId { get; }

    public Guid CustomerId { get; }

    public Guid? ContactId { get; }

    public string Title { get; }

    public string Description { get; }

    public IncidentCaseTypeCode CaseTypeCode { get; }

    public IncidentPriorityCode PriorityCode { get; }

    public Guid CallerUserId { get; }

    public long? SenderTelegramId { get; init; }

    public FlatArray<PictureModel> Pictures { get; init; }
}