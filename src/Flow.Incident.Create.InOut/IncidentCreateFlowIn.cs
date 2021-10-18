using System;
using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support.Bot;

public sealed record IncidentCreateFlowIn
{
    public IncidentCreateFlowIn(
        Guid ownerId, 
        Guid customerId, 
        [AllowNull] string customerTitle, 
        [AllowNull] string title, 
        [AllowNull] string description,
        int caseTypeCode)
    {
        OwnerId = ownerId;
        CustomerId = customerId;
        CustomerTitle = customerTitle ?? string.Empty;
        Title = title ?? string.Empty;
        Description = description ?? string.Empty;
        CaseTypeCode = caseTypeCode;
    }

    public Guid OwnerId { get; }

    public Guid CustomerId { get; }

    public string CustomerTitle { get; }

    public string Title { get; }

    public string Description { get; }

    public int CaseTypeCode {  get; }
}
